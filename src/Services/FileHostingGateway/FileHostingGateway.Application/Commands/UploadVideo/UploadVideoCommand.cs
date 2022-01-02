using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

using MediatR;

using FileHostingGateway.Application.Common.Interfaces;
using FileHostingGateway.Application.Common.Results;

namespace FileHostingGateway.Application.Commands.UploadVideo {
    public class UploadVideoCommand : IRequest<HandleResult<string>> {
        public string FilePath { get; init; }
        public string VimeoProjectId { get; init; }
    }

    public class UploadVideoCommandHandler : IRequestHandler<UploadVideoCommand, HandleResult<string>> {
        private readonly ILogger<UploadVideoCommandHandler> _logger;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IVimeoGateway _vimeoGateway;

        public UploadVideoCommandHandler(
            ILogger<UploadVideoCommandHandler> logger,
            IHostEnvironment hostEnvironment,
            IVimeoGateway vimeoGateway
        ) {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _vimeoGateway = vimeoGateway;
        }

        public async Task<HandleResult<string>> Handle(
            UploadVideoCommand command, CancellationToken cancellationToken
        ) {
            try {
                var outcome = await _vimeoGateway.UploadVideo(command.FilePath, command.VimeoProjectId);
                if (outcome.IsError) {
                    _logger.LogError(outcome.Error.Errors.Values.First().First());

                    return new HandleResult<string> {
                        Error = outcome.Error
                    };
                }

                return new HandleResult<string> {
                    Data = outcome.Data
                };
            } finally {
                if (!_hostEnvironment.IsDevelopment()) {
                    File.Delete(command.FilePath);
                }
            }
        }
    }
}
