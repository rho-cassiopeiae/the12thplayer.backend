using System.Linq;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using MessageBus.Contracts.Commands.Admin;

using Worker.Application.Commands.ExecuteOneOffJobs;
using Worker.Application.Commands.SchedulePeriodicJobs;
using Worker.Application.Options;

namespace Worker.Host.Consumers.Admin {
    public class JobCommandsConsumer :
        IConsumer<ExecuteOneOffJobs>,
        IConsumer<SchedulePeriodicJobs> {
        private readonly ISender _mediator;

        public JobCommandsConsumer(ISender mediator) {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ExecuteOneOffJobs> context) {
            var command = new ExecuteOneOffJobsCommand {
                Jobs = context.Message.Jobs
                    .Select(job => new JobOptions {
                        Name = job.Name,
                        Type = job.Type,
                        DataMap = job.DataMap,
                        ExecuteAfter = job.ExecuteAfter
                    })
                    .ToList()
            };

            await _mediator.Send(command);
        }

        public async Task Consume(ConsumeContext<SchedulePeriodicJobs> context) {
            var command = new SchedulePeriodicJobsCommand {
                Jobs = context.Message.Jobs
                    .Select(job => new JobOptions {
                        Name = job.Name,
                        Type = job.Type,
                        CronSchedule = job.CronSchedule,
                        DataMap = job.DataMap
                    })
                    .ToList()
            };

            await _mediator.Send(command);
        }
    }
}
