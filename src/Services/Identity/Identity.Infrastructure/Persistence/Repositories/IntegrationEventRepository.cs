using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Identity.Application.Common.Integration;
using Identity.Domain.Base;

namespace Identity.Infrastructure.Persistence.Repositories {
    public class IntegrationEventRepository : IIntegrationEventRepository {
        private readonly IntegrationEventDbContext _integrationEventDbContext;

        private IUnitOfWork _unitOfWork;

        public IntegrationEventRepository(
            IntegrationEventDbContext integrationEventDbContext
        ) {
            _integrationEventDbContext = integrationEventDbContext;
        }

        public void EnlistAsPartOf(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _integrationEventDbContext.Database.SetDbConnection(unitOfWork.Connection);
            _integrationEventDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _integrationEventDbContext.SaveChangesAsync(cancellationToken);
        }

        public void Create(IntegrationEvent @event) {
            _integrationEventDbContext.IntegrationEvents.Add(@event);
        }
    }
}
