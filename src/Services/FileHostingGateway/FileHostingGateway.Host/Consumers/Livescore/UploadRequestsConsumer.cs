using System;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using MessageBus.Contracts.Requests.Livescore;
using MessageBus.Contracts.Responses.FileHostingGateway;

using FileHostingGateway.Application.Commands.UploadVideo;

namespace FileHostingGateway.Host.Consumers.Livescore {
    public class UploadRequestsConsumer : IConsumer<UploadVideo> {
        private readonly ISender _mediator;

        public UploadRequestsConsumer(ISender mediator) {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UploadVideo> context) {
            var command = new UploadVideoCommand {
                FilePath = context.Message.FilePath,
                VimeoProjectId = context.Message.VimeoProjectId
            };

            var result = await _mediator.Send(command);
            if (result.Error != null) {
                throw new Exception(result.Error.Errors.Values.First().First());
            }

            await context.RespondAsync(new UploadVideoSuccess {
                CorrelationId = Guid.NewGuid(),
                VideoId = result.Data.VideoId,
                ThumbnailUrl = result.Data.ThumbnailUrl
            });
        }
    }
}
