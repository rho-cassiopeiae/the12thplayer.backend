using System;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using MessageBus.Contracts.Requests.Worker;
using MessageBus.Contracts.Responses.FileHostingGateway;

using FileHostingGateway.Application.Commands.AddFileFoldersForFixture;

namespace FileHostingGateway.Host.Consumers.Worker {
    public class FolderRequestsConsumer : IConsumer<AddFileFoldersForFixture> {
        private readonly ISender _mediator;

        public FolderRequestsConsumer(ISender mediator) {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<AddFileFoldersForFixture> context) {
            var command = new AddFileFoldersForFixtureCommand {
                FixtureId = context.Message.FixtureId,
                TeamId = context.Message.TeamId
            };

            var result = await _mediator.Send(command);
            if (result.Error != null) {
                // @@NOTE: Transient errors are handled by Polly, which means that any error propagating
                // up here is a serious one and should be resolved manually.
                throw new Exception(result.Error.Errors.Values.First().First());
            }

            await context.RespondAsync(new AddFileFoldersForFixtureSuccess {
                CorrelationId = Guid.NewGuid(),
                VimeoProjectId = result.Data
            });
        }
    }
}
