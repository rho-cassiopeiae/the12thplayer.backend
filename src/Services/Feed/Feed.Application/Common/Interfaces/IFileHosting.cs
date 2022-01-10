using System.Threading.Tasks;

namespace Feed.Application.Common.Interfaces {
    public interface IFileHosting {
        Task<string> UploadImage(string filePath);
    }
}
