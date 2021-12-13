using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Livescore.Application.Common.Errors;
using Livescore.Application.Common.Results;

namespace Livescore.Application.Common.Interfaces {
    public interface IFileReceiver {
        Task<Either<HandleError, FormCollection>> ReceiveImageAndFormValues(
            HttpRequest request, int maxSize, string filePrefix, string fileName = null
        );

        Task<Either<HandleError, FormCollection>> ReceiveVideoAndFormValues(
            HttpRequest request, int maxSize, string filePrefix, string fileName = null
        );

        void DeleteFile(string filePath);
    }
}
