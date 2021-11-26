using System;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Account {
    public interface IIntegrationEventPublisher : IAsyncDisposable {
        Task FetchAndPublishEventById(int eventId);
    }
}
