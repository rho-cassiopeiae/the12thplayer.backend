using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.VideoReaction {
    public interface IVideoReactionInMemRepository : IInMemRepository<VideoReaction> {
        void SetVimeoProjectIdFor(long fixtureId, long teamId, string vimeoProjectId);

        Task<bool> TryReserveSlotFor__immediate(long fixtureId, long teamId, long userId);

        Task ReleaseSlotFor__immediate(long fixtureId, long teamId, long userId);

        void Create(VideoReaction videoReaction);

        void UpdateRatingFor(long fixtureId, long teamId, long authorId, int incrementRatingBy);

        void DeleteAllFor(long fixtureId, long teamId);
    }
}
