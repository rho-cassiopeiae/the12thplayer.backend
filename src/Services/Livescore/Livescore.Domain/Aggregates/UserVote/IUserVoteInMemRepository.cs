using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.UserVote {
    public interface IUserVoteInMemRepository : IInMemRepository<UserVote> {
        Task<UserVote> FindOneForFixtureParticipant(
            long userId, long fixtureId, long teamId, string participantKey
        );

        Task<IEnumerable<UserVote>> FindAllFor(
            long fixtureId, long teamId,
            List<string> fixtureParticipantKeys
        );

        Task<UserVote> FindOneForVideoReaction(
            long userId, long fixtureId, long teamId, long authorId
        );

        void UpdateOneForFixtureParticipant(UserVote userVote, float? oldRating);

        void UpdateOneForVideoReaction(UserVote userVote, short? oldVote);

        void DeleteAllFor(
            long fixtureId, long teamId,
            List<string> fixtureParticipantKeys
        );
    }
}
