using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Quartz;

using Worker.Application.Common.Errors;
using Worker.Application.Common.Results;
using Worker.Application.Jobs.OneOff;
using Worker.Application.Options;

namespace Worker.Application.Commands.ExecuteOneOffJobs {
    public class ExecuteOneOffJobsCommand : IRequest<VoidResult> {
        public List<JobOptions> Jobs { get; init; }
    }

    public class ExecuteOneOffJobsCommandHandler : IRequestHandler<
        ExecuteOneOffJobsCommand, VoidResult
    > {
        private readonly ISchedulerFactory _schedulerFactory;

        public ExecuteOneOffJobsCommandHandler(ISchedulerFactory schedulerFactory) {
            _schedulerFactory = schedulerFactory;
        }

        public async Task<VoidResult> Handle(
            ExecuteOneOffJobsCommand command, CancellationToken cancellationToken
        ) {
            // @@TODO: Make sure that the dependency graph is possible.

            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            var jobs = command.Jobs;

            if (jobs.Count == 0) {
                return new VoidResult {
                    Error = new ValidationError("No jobs specified")
                };
            }

            foreach (var job in jobs) {
                var type = Type.GetType(job.Type);
                if (type == null) {
                    return new VoidResult {
                        Error = new ValidationError(
                            $"Job {job.Type} does not exist"
                        )
                    };
                } else if (!type.IsSubclassOf(typeof(OneOffJob))) {
                    return new VoidResult {
                        Error = new ValidationError(
                            $"Job {job.Type} is not a one-off job"
                        )
                    };
                }
            }

            var jobToDependantJobs = new Dictionary<int, List<int>>();
            for (int i = 0; i < jobs.Count; ++i) {
                var job = jobs[i];
                for (int j = i + 1; j < jobs.Count; ++j) {
                    var belowJob = jobs[j];
                    if (belowJob.ExecuteAfter == job.Name) {
                        if (
                            jobToDependantJobs.TryGetValue(
                                i, out List<int> dependantJobs
                            )
                        ) {
                            dependantJobs.Add(j);
                        } else {
                            jobToDependantJobs[i] = new List<int> { j };
                        }
                    }
                }
            }

            var uuids = jobs.Select(_ => Guid.NewGuid().ToString()).ToList();
            for (int i = 0; i < jobs.Count; ++i) {
                var job = jobs[i];

                var createMethod = typeof(JobBuilder)
                    .GetMethod(
                        "Create",
                        1,
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                        Type.EmptyTypes,
                        null
                    )
                    .MakeGenericMethod(Type.GetType(job.Type));

                var jobBuilder = (JobBuilder) createMethod.Invoke(null, null);
                jobBuilder
                    .WithIdentity(uuids[i])
                    .StoreDurably(true);

                var jobDataMap = job.DataMap != null ?
                    new JobDataMap(
                        (IDictionary<string, object>)
                        new Dictionary<string, object>(
                            job.DataMap.Select(kv =>
                                new KeyValuePair<string, object>(
                                    kv.Key, kv.Value
                                )
                            )
                        )
                    ) :
                    new JobDataMap();

                if (jobToDependantJobs.TryGetValue(i, out List<int> dependantJobs)) {
                    jobDataMap.Add(
                        "NEXT ==>",
                        string.Join(',', dependantJobs.Select(index => uuids[index]))
                    );
                }

                var jobDetail = jobBuilder
                    .UsingJobData(jobDataMap)
                    .Build();

                await scheduler.AddJob(jobDetail, replace: false);
            }

            var trigger = TriggerBuilder.Create()
                .ForJob(uuids[0])
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(trigger);

            return VoidResult.Instance;
        }
    }
}
