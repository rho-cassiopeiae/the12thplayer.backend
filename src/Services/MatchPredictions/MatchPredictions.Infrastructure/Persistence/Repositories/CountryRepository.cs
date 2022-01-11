using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MatchPredictions.Domain.Aggregates.Country;

namespace MatchPredictions.Infrastructure.Persistence.Repositories {
    public class CountryRepository : ICountryRepository {
        private readonly MatchPredictionsDbContext _matchPredictionsDbContext;

        public CountryRepository(MatchPredictionsDbContext matchPredictionsDbContext) {
            _matchPredictionsDbContext = matchPredictionsDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _matchPredictionsDbContext.SaveChangesAsync(cancellationToken);
        }

        public void Create(IEnumerable<Country> countries) {
            _matchPredictionsDbContext.Countries.AddRange(countries);
        }
    }
}
