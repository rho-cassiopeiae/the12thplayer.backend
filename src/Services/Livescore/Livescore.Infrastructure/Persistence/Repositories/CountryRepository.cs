using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Livescore.Domain.Aggregates.Country;

namespace Livescore.Infrastructure.Persistence.Repositories {
    public class CountryRepository : ICountryRepository {
        private readonly LivescoreDbContext _livescoreDbContext;

        public CountryRepository(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _livescoreDbContext.SaveChangesAsync(cancellationToken);
        }

        public void Create(IEnumerable<Country> countries) {
            _livescoreDbContext.Countries.AddRange(countries);
        }
    }
}
