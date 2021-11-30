using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Worker.Application.Jobs.Periodic {
    public abstract class PeriodicJob : IJob {
        protected readonly ILogger _logger;
        protected IJobExecutionContext _context;

        protected PeriodicJob(ILogger logger) {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context) {
            _context = context;

            _logger.LogInformation("Job started");

            await _execute();

            _logger.LogInformation("Job finished");
        }

        protected abstract Task _execute();

        protected bool _jobCanceled =>
            _context.CancellationToken.IsCancellationRequested;
    }
}
