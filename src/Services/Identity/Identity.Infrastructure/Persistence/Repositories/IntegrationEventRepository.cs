using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Identity.Application.Common.Integration;
using Identity.Domain.Base;

namespace Identity.Infrastructure.Persistence.Repositories {
    public class IntegrationEventRepository : IIntegrationEventRepository {
        private readonly IntegrationEventDbContext _integrationEventDbContext;

        public IntegrationEventRepository(
            IntegrationEventDbContext integrationEventDbContext
        ) {
            _integrationEventDbContext = integrationEventDbContext;
        }

        public void EnlistAsPartOf(IUnitOfWork unitOfWork) {
            _integrationEventDbContext.Database.SetDbConnection(unitOfWork.Connection);
            _integrationEventDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken) {
            await _integrationEventDbContext.SaveChangesAsync(cancellationToken);
        }

        public void Create(IntegrationEvent @event) {
            _integrationEventDbContext.IntegrationEvents.Add(@event);
        }
    }
}
