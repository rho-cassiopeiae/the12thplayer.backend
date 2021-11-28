using System;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Integration {
    public interface IIntegrationEventPublisher : IAsyncDisposable {
        Task FetchAndPublishPendingEvents();
        Task FetchAndPublishEventById(Guid eventId);
    }
}
