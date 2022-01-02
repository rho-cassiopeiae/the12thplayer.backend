using System;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using MessageBus.Contracts.Requests.Profile;
using MessageBus.Contracts.Responses.FileHostingGateway;

using FileHostingGateway.Application.Commands.UploadImage;

namespace FileHostingGateway.Host.Consumers {
    public partial class UploadRequestsConsumer : IConsumer<UploadImage> {
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
