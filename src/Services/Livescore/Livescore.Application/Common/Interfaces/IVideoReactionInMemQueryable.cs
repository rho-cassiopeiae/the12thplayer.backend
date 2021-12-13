using System.Threading.Tasks;

namespace Livescore.Application.Common.Interfaces {
    public interface IVideoReactionInMemQueryable {
        Task<string> GetVimeoProjectIdFor(long fixtureId, long teamId);
        Task<int> GetRatingFor(long fixtureId, long teamId, long authorId);
    }
}
