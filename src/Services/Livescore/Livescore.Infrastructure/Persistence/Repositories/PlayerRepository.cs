using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.Player;

namespace Livescore.Infrastructure.Persistence.Repositories {
    public class PlayerRepository : IPlayerRepository {
        private readonly LivescoreDbContext _livescoreDbContext;

        public PlayerRepository(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _livescoreDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Player>> FindById(IEnumerable<long> ids) {
            var players = await _livescoreDbContext.Players
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            return players;
        }

        public void Create(Player player) {
            _livescoreDbContext.Players.Add(player);
        }
    }
}
