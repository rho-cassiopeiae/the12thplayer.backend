using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using FileHostingGateway.Application.Common.Interfaces;

namespace FileHostingGateway.Infrastructure.S3 {
    public class S3GatewayMock : IS3Gateway {
        private readonly string _profileServiceAddress;
        private readonly string _livescoreServiceAddress;
        private readonly string _feedServiceAddress;

        public S3GatewayMock(IConfiguration configuration) {
            _profileServiceAddress = configuration["Profile:Address"];
            _livescoreServiceAddress = configuration["Livescore:Address"];
            _feedServiceAddress = configuration["Feed:Address"];
        }

        public Task<string> UploadImage(string filePath) {
            // wwwroot/user-files/**/image.png
            return Task.FromResult(
                (
                    filePath.Contains("profile-images") ?
                        _profileServiceAddress :
                        filePath.Contains("video-thumbnails") ?
                            _feedServiceAddress : _livescoreServiceAddress
                ) +
                filePath.Substring(7)
            );
        }
    }
}
