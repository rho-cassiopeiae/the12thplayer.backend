using System.Threading.Tasks;

namespace FileHostingGateway.Application.Common.Interfaces {
    public interface IS3Gateway {
        Task<string> UploadImage(string filePath);
    }
}
