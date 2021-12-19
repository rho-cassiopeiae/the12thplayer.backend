using System.Threading.Tasks;

namespace Profile.Application.Common.Interfaces {
    public interface IFileHosting {
        Task<string> UploadImage(string filePath);
    }
}
