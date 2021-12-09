using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.PlayerRating;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.Persistence.Queryables {
    public class PlayerRatingQueryable : IPlayerRatingQueryable {
        private readonly LivescoreDbContext _livescoreDbContext;

        public PlayerRatingQueryable(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task<IEnumerable<PlayerRating>> GetAllFor(long fixtureId, long teamId) {
            var playerRatings = await _livescoreDbContext.PlayerRatings
                .Where(pr => pr.FixtureId == fixtureId && pr.TeamId == teamId)
                .Select(pr => new PlayerRating(
                    fixtureId, teamId, pr.ParticipantKey, pr.TotalRating, pr.TotalVoters
                ))
                .ToListAsync();

            return playerRatings;
        }
    }
}
