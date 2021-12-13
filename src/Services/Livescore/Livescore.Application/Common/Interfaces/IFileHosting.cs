using System.Threading.Tasks;

namespace Livescore.Application.Common.Interfaces {
    public interface IFileHosting {
        Task<(string VideoId, string ThumbnailUrl)> UploadVideo(
            string filePath, string vimeoProjectId
        );
    }
}
