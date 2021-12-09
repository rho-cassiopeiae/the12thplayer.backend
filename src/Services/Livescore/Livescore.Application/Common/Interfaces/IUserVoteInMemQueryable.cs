using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Aggregates.UserVote;

namespace Livescore.Application.Common.Interfaces {
    public interface IUserVoteInMemQueryable {
        Task<UserVote> GetPlayerRatingsFor(
            long userId, long fixtureId, long teamId,
            List<string> fixtureParticipantKeys
        );
    }
}
