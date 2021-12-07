using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.VideoReaction {
    public interface IVideoReactionInMemRepository : IInMemRepository<VideoReaction> {
        void UpdateRatingFor(long fixtureId, long teamId, long authorId, int incrementRatingBy);
    }
}
