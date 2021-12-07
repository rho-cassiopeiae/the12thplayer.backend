using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.UserVote {
    public interface IUserVoteInMemRepository : IInMemRepository<UserVote> {
        Task<UserVote> FindOneForFixtureParticipant(
            long userId, long fixtureId, long teamId, string participantKey
        );

        Task<UserVote> FindOneForVideoReaction(
            long userId, long fixtureId, long teamId, long authorId
        );

        void UpdateOneForFixtureParticipant(UserVote userVote, float? oldRating);

        void UpdateOneForVideoReaction(UserVote userVote, short? oldVote);
    }
}
