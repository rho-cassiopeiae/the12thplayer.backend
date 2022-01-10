using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Feed.Application.Common.Errors;
using Feed.Application.Common.Results;

namespace Feed.Application.Common.Interfaces {
    public interface IFileReceiver {
        Task<Either<HandleError, FormCollection>> ReceiveImageAndFormValues(
            HttpRequest request, int maxSize, string filePrefix, string fileName = null
        );

        void DeleteFile(string filePath);
    }
}
