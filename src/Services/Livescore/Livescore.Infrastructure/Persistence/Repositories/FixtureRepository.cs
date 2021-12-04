using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.Fixture;

namespace Livescore.Infrastructure.Persistence.Repositories {
    public class FixtureRepository : IFixtureRepository {
        private readonly LivescoreDbContext _livescoreDbContext;

        public FixtureRepository(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _livescoreDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Fixture> FindByKey(long id, long teamId) {
            var fixture = await _livescoreDbContext.Fixtures.SingleAsync(
                f => f.Id == id && f.TeamId == teamId
            );

            return fixture;
        }

        public async Task<IEnumerable<Fixture>> FindByTeamId(long teamId) {
            var fixtures = await _livescoreDbContext.Fixtures
                .AsNoTracking()
                .Where(f => f.TeamId == teamId)
                .ToListAsync();

            return fixtures;
        }

        public void Create(Fixture fixture) {
            _livescoreDbContext.Fixtures.Add(fixture);
        }
    }
}
