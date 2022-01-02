using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.Venue;

namespace Livescore.Infrastructure.Persistence.Repositories {
    public class VenueRepository : IVenueRepository {
        private readonly LivescoreDbContext _livescoreDbContext;

        public VenueRepository(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _livescoreDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<Venue> FindById(long id) => _livescoreDbContext.Venues.SingleOrDefaultAsync(v => v.Id == id);

        public async Task<IEnumerable<Venue>> FindById(IEnumerable<long> ids) {
            var venues = await _livescoreDbContext.Venues
                .AsNoTracking()
                .Where(v => ids.Contains(v.Id))
                .ToListAsync();

            return venues;
        }

        public void Create(Venue venue) {
            _livescoreDbContext.Venues.Add(venue);
        }
    }
}
