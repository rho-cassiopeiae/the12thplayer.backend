using System.Threading.Tasks;

using FileHostingGateway.Application.Commands.UploadVideo;
using FileHostingGateway.Application.Common.Errors;
using FileHostingGateway.Application.Common.Results;

namespace FileHostingGateway.Application.Common.Interfaces {
    public interface IVimeoGateway {
        Task<Either<VimeoError, string>> AddProjectFor(long fixtureId, long teamId);

        Task<Either<VimeoError, VideoStreamingInfoDto>> UploadVideo(
            string filePath, string projectId
        );
    }
}
