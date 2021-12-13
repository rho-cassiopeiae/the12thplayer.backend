using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Application.Livescore.VideoReaction.Queries.GetVideoReactionsForFixture;
using Livescore.Domain.Aggregates.VideoReaction;

namespace Livescore.Application.Common.Interfaces {
    public interface IVideoReactionInMemQueryable {
        Task<string> GetVimeoProjectIdFor(long fixtureId, long teamId);

        Task<IEnumerable<VideoReaction>> GetAllFor(
            long fixtureId, long teamId, VideoReactionFilter filter, int start
        );

        Task<int> GetRatingFor(long fixtureId, long teamId, long authorId);
    }
}
