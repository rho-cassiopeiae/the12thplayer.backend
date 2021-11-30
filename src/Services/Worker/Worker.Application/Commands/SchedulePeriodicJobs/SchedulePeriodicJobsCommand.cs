using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MediatR;
using Quartz;

using Worker.Application.Common.Results;
using Worker.Application.Options;
using Worker.Application.Common.Errors;
using Worker.Application.Jobs.Periodic;

namespace Worker.Application.Commands.SchedulePeriodicJobs {
    public class SchedulePeriodicJobsCommand : IRequest<VoidResult> {
        public List<JobOptions> Jobs { get; init; }
    }

    public class SchedulePeriodicJobsCommandHandler : IRequestHandler<
        SchedulePeriodicJobsCommand, VoidResult
    > {
        private readonly ILogger<SchedulePeriodicJobsCommandHandler> _logger;
        private readonly ISchedulerFactory _schedulerFactory;

        public SchedulePeriodicJobsCommandHandler(
            ILogger<SchedulePeriodicJobsCommandHandler> logger,
            ISchedulerFactory schedulerFactory
        ) {
            _logger = logger;
            _schedulerFactory = schedulerFactory;
        }

        public async Task<VoidResult> Handle(
            SchedulePeriodicJobsCommand command, CancellationToken cancellationToken
        ) {
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
                } else if (!type.IsSubclassOf(typeof(PeriodicJob))) {
                    return new VoidResult {
                        Error = new ValidationError(
                            $"Job {job.Type} is not a periodic job"
                        )
                    };
                }
            }

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

                var jobDetail = ((JobBuilder) createMethod.Invoke(null, null))
                    .WithIdentity(job.Name)
                    .UsingJobData(
                        job.DataMap != null ?
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
                            new JobDataMap()
                    )
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithCronSchedule(
                        job.CronSchedule,
                        scheduleBuilder =>
                            scheduleBuilder.InTimeZone(TimeZoneInfo.Utc)
                    )
                    .StartNow()
                    .Build();

                DateTimeOffset firstExecutionAt = await scheduler.ScheduleJob(
                    jobDetail, trigger
                );

                _logger.LogInformation(
                    "Job {Job}: Scheduled. First execution at {FirstExecutionAt}",
                    job.Name, firstExecutionAt
                );
            }

            return VoidResult.Instance;
        }
    }
}
