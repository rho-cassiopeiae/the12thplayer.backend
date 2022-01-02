using System.Threading.Tasks;

using Livescore.Application.Common.Interfaces;

namespace Livescore.IntegrationTests.Livescore.VideoReaction.Mocks {
    public class FileHostingMock : IFileHosting {
        public static readonly string VideoId = "videoId";

        public Task<string> UploadVideo(string filePath, string vimeoProjectId) {
            return Task.FromResult(VideoId);
        }
    }
}
