using System.Threading.Tasks;

using FileHostingGateway.Application.Common.Errors;
using FileHostingGateway.Application.Common.Results;

namespace FileHostingGateway.Application.Common.Interfaces {
    public interface IVimeoGateway {
        Task<Either<VimeoError, string>> AddProjectFor(long fixtureId, long teamId);
        Task<Either<VimeoError, string>> UploadVideo(string filePath, string projectId);
    }
}
