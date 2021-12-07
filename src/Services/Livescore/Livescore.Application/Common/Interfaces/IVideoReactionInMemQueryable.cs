using System.Threading.Tasks;

namespace Livescore.Application.Common.Interfaces {
    public interface IVideoReactionInMemQueryable {
        Task<int> GetRatingFor(long fixtureId, long teamId, long authorId);
    }
}
