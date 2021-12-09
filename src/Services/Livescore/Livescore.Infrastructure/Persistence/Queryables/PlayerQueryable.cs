using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.Player;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.Persistence.Queryables {
    public class PlayerQueryable : IPlayerQueryable {
        private readonly LivescoreDbContext _livescoreDbContext;

        public PlayerQueryable(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task<IEnumerable<Player>> GetPlayersFrom(long teamId) {
            var players = await _livescoreDbContext.Players
                .AsNoTracking()
                .Where(p => p.TeamId == teamId)
                .ToListAsync();

            return players;
        }
    }
}
