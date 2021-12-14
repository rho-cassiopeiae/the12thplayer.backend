using System;
using System.Threading.Tasks;

namespace Profile.Infrastructure.Integration {
    public interface IIntegrationEventPublisher : IAsyncDisposable {
        Task FetchAndPublishPendingEvents();
        Task FetchAndPublishEventById(Guid eventId);
    }
}
