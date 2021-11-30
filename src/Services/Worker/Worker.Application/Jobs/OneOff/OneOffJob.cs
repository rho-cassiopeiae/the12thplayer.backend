using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Worker.Application.Jobs.OneOff {
    public abstract class OneOffJob : IJob {
        protected readonly ILogger _logger;
        protected IJobExecutionContext _context;

        protected OneOffJob(ILogger logger) {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context) {
            _context = context;

            _logger.LogInformation("Job started");

            IDictionary<string, object> nextJobDataMap = await _execute();

            _logger.LogInformation("Job finished");

            if (
                context.MergedJobDataMap.TryGetValue(
                    "NEXT ==>", out object value
                )
            ) {
                var nextJobNames = ((string) value).Split(',');
                foreach (var nextJobName in nextJobNames) {
                    var triggerBuilder = TriggerBuilder.Create()
                        .ForJob(nextJobName)
                        .StartNow();

                    if (nextJobDataMap != null) {
                        triggerBuilder.UsingJobData(new JobDataMap(nextJobDataMap));
                    }

                    await context.Scheduler.ScheduleJob(triggerBuilder.Build());
                }
            }

            await context.Scheduler.DeleteJob(context.JobDetail.Key);
        }

        protected abstract Task<IDictionary<string, object>> _execute();

        protected bool _isCanceled =>
            _context.CancellationToken.IsCancellationRequested;
    }
}
