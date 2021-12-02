using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.Manager;

namespace Livescore.Infrastructure.Persistence.Repositories {
    public class ManagerRepository : IManagerRepository {
        private readonly LivescoreDbContext _livescoreDbContext;

        public ManagerRepository(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _livescoreDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Manager> FindById(long id) {
            var manager = await _livescoreDbContext.Managers.SingleOrDefaultAsync(
                m => m.Id == id
            );

            return manager;
        }

        public void Create(Manager manager) {
            _livescoreDbContext.Managers.Add(manager);
        }
    }
}
