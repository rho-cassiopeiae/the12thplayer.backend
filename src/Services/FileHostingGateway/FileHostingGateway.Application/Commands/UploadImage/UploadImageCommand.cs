using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using MediatR;

using FileHostingGateway.Application.Common.Interfaces;
using FileHostingGateway.Application.Common.Results;

namespace FileHostingGateway.Application.Commands.UploadImage {
    public class UploadImageCommand : IRequest<HandleResult<string>> {
        public string FilePath { get; init; }
    }

    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, HandleResult<string>> {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IS3Gateway _s3Gateway;

        public UploadImageCommandHandler(
            IHostEnvironment hostEnvironment,
            IS3Gateway s3Gateway
        ) {
            _hostEnvironment = hostEnvironment;
            _s3Gateway = s3Gateway;
        }

        public async Task<HandleResult<string>> Handle(
            UploadImageCommand command, CancellationToken cancellationToken
        ) {
            try {
                return new HandleResult<string> {
                    Data = await _s3Gateway.UploadImage(command.FilePath)
                };
            } finally {
                if (!_hostEnvironment.IsDevelopment()) {
                    File.Delete(command.FilePath);
                }
            }
        }
    }
}
