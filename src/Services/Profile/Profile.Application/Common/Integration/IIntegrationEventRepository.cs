using Profile.Domain.Base;

namespace Profile.Application.Common.Integration {
    public interface IIntegrationEventRepository : IRepository<IntegrationEvent> {
        void Create(IntegrationEvent @event);
    }
}
