using System.Threading.Tasks;

namespace Livescore.Application.Common.Interfaces {
    public interface IFileHosting {
        Task<string> UploadVideo(string filePath, string vimeoProjectId);
    }
}
