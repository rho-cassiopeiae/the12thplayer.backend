using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MediatR;

using FileHostingGateway.Application.Common.Interfaces;
using FileHostingGateway.Application.Common.Results;

namespace FileHostingGateway.Application.Commands.UploadVideo {
    public class UploadVideoCommand : IRequest<HandleResult<VideoStreamingInfoDto>> {
        public string FilePath { get; init; }
        public string VimeoProjectId { get; init; }
    }

    public class UploadVideoCommandHandler : IRequestHandler<
        UploadVideoCommand, HandleResult<VideoStreamingInfoDto>
    > {
        private readonly ILogger<UploadVideoCommandHandler> _logger;
        private readonly IVimeoGateway _vimeoGateway;

        public UploadVideoCommandHandler(
            ILogger<UploadVideoCommandHandler> logger,
            IVimeoGateway vimeoGateway
        ) {
            _logger = logger;
            _vimeoGateway = vimeoGateway;
        }

        public async Task<HandleResult<VideoStreamingInfoDto>> Handle(
            UploadVideoCommand command, CancellationToken cancellationToken
        ) {
            try {
                var outcome = await _vimeoGateway.UploadVideo(command.FilePath, command.VimeoProjectId);
                if (outcome.IsError) {
                    _logger.LogError(outcome.Error.Errors.Values.First().First());

                    return new HandleResult<VideoStreamingInfoDto> {
                        Error = outcome.Error
                    };
                }

                return new HandleResult<VideoStreamingInfoDto> {
                    Data = outcome.Data
                };
            } finally {
                File.Delete(command.FilePath);
            }
        }
    }
}
