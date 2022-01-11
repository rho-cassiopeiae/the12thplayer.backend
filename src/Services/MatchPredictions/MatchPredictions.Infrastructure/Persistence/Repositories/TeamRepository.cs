using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MatchPredictions.Domain.Aggregates.Team;

namespace MatchPredictions.Infrastructure.Persistence.Repositories {
    public class TeamRepository : ITeamRepository {
        private readonly MatchPredictionsDbContext _matchPredictionsDbContext;

        public TeamRepository(MatchPredictionsDbContext matchPredictionsDbContext) {
            _matchPredictionsDbContext = matchPredictionsDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _matchPredictionsDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Team>> FindById(IEnumerable<long> ids) {
            var teams = await _matchPredictionsDbContext.Teams
                .AsNoTracking()
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            return teams;
        }

        public void Create(Team team) {
            _matchPredictionsDbContext.Teams.Add(team);
        }
    }
}
