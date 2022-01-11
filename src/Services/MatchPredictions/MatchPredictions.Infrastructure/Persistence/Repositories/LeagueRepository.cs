using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MatchPredictions.Domain.Aggregates.League;

namespace MatchPredictions.Infrastructure.Persistence.Repositories {
    public class LeagueRepository : ILeagueRepository {
        private readonly MatchPredictionsDbContext _matchPredictionsDbContext;

        public LeagueRepository(MatchPredictionsDbContext matchPredictionsDbContext) {
            _matchPredictionsDbContext = matchPredictionsDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _matchPredictionsDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<League> FindById(long id) =>
            _matchPredictionsDbContext.Leagues
                .Include(l => l.Seasons)
                .SingleOrDefaultAsync(l => l.Id == id);

        public void Create(League league) {
            _matchPredictionsDbContext.Leagues.Add(league);
        }
    }
}