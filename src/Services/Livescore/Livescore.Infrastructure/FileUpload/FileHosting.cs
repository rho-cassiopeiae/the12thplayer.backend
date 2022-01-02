using System;
using System.Threading.Tasks;

using MassTransit;

using MessageBus.Contracts.Requests.Livescore;
using MessageBus.Contracts.Responses.FileHostingGateway;

using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.FileUpload {
    public class FileHosting : IFileHosting {
        private readonly IRequestClient<UploadVideo> _uploadVideoClient;

        public FileHosting(IRequestClient<UploadVideo> uploadVideoClient) {
            _uploadVideoClient = uploadVideoClient;
        }

        public async Task<string> UploadVideo(string filePath, string vimeoProjectId) {
            var response = await _uploadVideoClient.GetResponse<UploadVideoSuccess>(
                new UploadVideo {
                    CorrelationId = Guid.NewGuid(),
                    FilePath = filePath,
                    VimeoProjectId = vimeoProjectId
                }
            );

            return response.Message.VideoId;
        }
    }
}
