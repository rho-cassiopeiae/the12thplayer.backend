using System;
using System.Threading.Tasks;

using MassTransit;

using MessageBus.Contracts.Requests.Feed;
using MessageBus.Contracts.Responses.FileHostingGateway;

using Feed.Application.Common.Interfaces;

namespace Feed.Infrastructure.FileUpload {
    public class FileHosting : IFileHosting {
        private readonly IRequestClient<UploadImage> _uploadImageClient;

        public FileHosting(IRequestClient<UploadImage> uploadImageClient) {
            _uploadImageClient = uploadImageClient;
        }

        public async Task<string> UploadImage(string filePath) {
            var response = await _uploadImageClient.GetResponse<UploadImageSuccess>(
                new UploadImage {
                    CorrelationId = Guid.NewGuid(),
                    FilePath = filePath
                }
            );

            return response.Message.ImageUrl;
        }
    }
}
