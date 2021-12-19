using System;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using MessageBus.Contracts.Requests.Profile;
using MessageBus.Contracts.Responses.FileHostingGateway;

using FileHostingGateway.Application.Commands.UploadImage;

namespace FileHostingGateway.Host.Consumers.Profile {
    public class ProfileUploadRequestsConsumer : IConsumer<UploadImage> {
        private readonly ISender _mediator;

        public ProfileUploadRequestsConsumer(ISender mediator) {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UploadImage> context) {
            var command = new UploadImageCommand {
                FilePath = context.Message.FilePath
            };

            var result = await _mediator.Send(command);
            if (result.Error != null) {
                throw new Exception(result.Error.Errors.Values.First().First());
            }

            await context.RespondAsync(new UploadImageSuccess {
                CorrelationId = Guid.NewGuid(),
                ImageUrl = result.Data
            });
        }
    }
}
