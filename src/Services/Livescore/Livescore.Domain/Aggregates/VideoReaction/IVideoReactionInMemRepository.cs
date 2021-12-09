using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.VideoReaction {
    public interface IVideoReactionInMemRepository : IInMemRepository<VideoReaction> {
        void Create(VideoReaction videoReaction);

        void UpdateRatingFor(long fixtureId, long teamId, long authorId, int incrementRatingBy);

        void DeleteAllFor(long fixtureId, long teamId);
    }
}
