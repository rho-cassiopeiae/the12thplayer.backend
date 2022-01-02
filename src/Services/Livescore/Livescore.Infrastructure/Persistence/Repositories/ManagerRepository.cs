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

        public Task<Manager> FindById(long id) => _livescoreDbContext.Managers.SingleOrDefaultAsync(m => m.Id == id);

        public void Create(Manager manager) {
            _livescoreDbContext.Managers.Add(manager);
        }
    }
}
