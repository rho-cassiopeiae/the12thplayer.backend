using System.Threading.Tasks;

using Livescore.Domain.Aggregates.UserVote;

namespace Livescore.Application.Common.Interfaces {
    public interface IUserVoteQueryable {
        Task<UserVote> GetPlayerRatingsFor(long userId, long fixtureId, long teamId);
    }
}
