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

        public async Task<Venue> FindById(long id) {
            var venue = await _livescoreDbContext.Venues.SingleOrDefaultAsync(
                v => v.Id == id
            );

            return venue;
        }

        public void Create(Venue venue) {
            _livescoreDbContext.Venues.Add(venue);
        }
    }
}
