using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.UserVote;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.Persistence.Queryables {
    public class UserVoteQueryable : IUserVoteQueryable {
        private readonly LivescoreDbContext _livescoreDbContext;

        public UserVoteQueryable(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task<UserVote> GetPlayerRatingsFor(long userId, long fixtureId, long teamId) {
            var result = await _livescoreDbContext.UserVotes
                .Where(uv => uv.UserId == userId && uv.FixtureId == fixtureId && uv.TeamId == teamId)
                .Select(uv => new {
                    FixtureParticipantKeyToRating = uv.FixtureParticipantKeyToRating
                })
                .SingleOrDefaultAsync();

            if (result?.FixtureParticipantKeyToRating == null) {
                return null;
            }

            var userVote = new UserVote(
                userId: userId,
                fixtureId: fixtureId,
                teamId: teamId
            );
            userVote.SetPlayerRatings(result.FixtureParticipantKeyToRating);

            return userVote;
        }
    }
}
