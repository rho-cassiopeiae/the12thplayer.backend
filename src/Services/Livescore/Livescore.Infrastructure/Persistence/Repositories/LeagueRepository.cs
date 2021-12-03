using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.League;

namespace Livescore.Infrastructure.Persistence.Repositories {
    public class LeagueRepository : ILeagueRepository {
        private readonly LivescoreDbContext _livescoreDbContext;

        public LeagueRepository(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _livescoreDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<League>> FindById(IEnumerable<long> ids) {
            var leagues = await _livescoreDbContext.Leagues
                .Include(l => l.Seasons)
                .Where(l => ids.Contains(l.Id))
                .ToListAsync();

            return leagues;
        }

        public void Create(League league) {
            _livescoreDbContext.Leagues.Add(league);
        }
    }
}