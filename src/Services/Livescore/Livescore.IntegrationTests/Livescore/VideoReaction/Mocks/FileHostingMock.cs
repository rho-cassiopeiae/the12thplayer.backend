using System.Threading.Tasks;

using Livescore.Application.Common.Interfaces;

namespace Livescore.IntegrationTests.Livescore.VideoReaction.Mocks {
    public class FileHostingMock : IFileHosting {
        public static readonly string VideoId = "videoId";
        public static readonly string ThumbnailUrl = "thumbnailUrl";

        public Task<(string VideoId, string ThumbnailUrl)> UploadVideo(string filePath, string vimeoProjectId) {
            return Task.FromResult((VideoId, ThumbnailUrl));
        }
    }
}
