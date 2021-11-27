using System;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Account {
    public interface IIntegrationEventPublisher : IAsyncDisposable {
        Task FetchAndPublishPendingEvents();
        Task FetchAndPublishEventById(Guid eventId);
    }
}
