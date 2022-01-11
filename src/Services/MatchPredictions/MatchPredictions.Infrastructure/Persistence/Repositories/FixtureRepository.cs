using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MatchPredictions.Domain.Aggregates.Fixture;

namespace MatchPredictions.Infrastructure.Persistence.Repositories {
    public class FixtureRepository : IFixtureRepository {
        private readonly MatchPredictionsDbContext _matchPredictionsDbContext;

        public FixtureRepository(MatchPredictionsDbContext matchPredictionsDbContext) {
            _matchPredictionsDbContext = matchPredictionsDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _matchPredictionsDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Fixture>> FindById(IEnumerable<long> ids) {
            var fixtures = await _matchPredictionsDbContext.Fixtures
                .Where(f => ids.Contains(f.Id))
                .ToListAsync();

            return fixtures;
        }

        public void Create(Fixture fixture) {
            _matchPredictionsDbContext.Fixtures.Add(fixture);
        }
    }
}
