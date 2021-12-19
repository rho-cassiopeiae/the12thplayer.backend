using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using FileHostingGateway.Application.Common.Interfaces;

namespace FileHostingGateway.Infrastructure.S3 {
    public class S3GatewayMock : IS3Gateway {
        private readonly string _profileServiceAddress;
        private readonly string _livescoreServiceAddress;

        public S3GatewayMock(IConfiguration configuration) {
            _profileServiceAddress = configuration["Profile:Address"];
            _livescoreServiceAddress = configuration["Livescore:Address"];
        }

        public Task<string> UploadImage(string filePath) {
            // wwwroot/user-files/**/image.png
            return Task.FromResult(
                (filePath.Contains("profile-images") ? _profileServiceAddress : _livescoreServiceAddress) +
                filePath.Substring(7)
            );
        }
    }
}
