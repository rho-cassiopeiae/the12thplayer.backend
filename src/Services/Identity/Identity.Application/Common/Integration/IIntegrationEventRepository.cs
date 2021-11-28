using Identity.Domain.Base;

namespace Identity.Application.Common.Integration {
    public interface IIntegrationEventRepository : IRepository<IntegrationEvent> {
        void Create(IntegrationEvent @event);
    }
}
