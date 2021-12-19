using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Profile.Application.Common.Errors;
using Profile.Application.Common.Results;

namespace Profile.Application.Common.Interfaces {
    public interface IFileReceiver {
        Task<Either<HandleError, FormCollection>> ReceiveImageAndFormValues(
            HttpRequest request, int maxSize, string filePrefix, string fileName
        );

        void DeleteFile(string filePath);
    }
}
