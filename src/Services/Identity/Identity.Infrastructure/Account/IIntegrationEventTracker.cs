using System;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Account {
    public interface IIntegrationEventTracker : IAsyncDisposable {
        Task ListenForAndPublishNewEvents(CancellationToken stoppingToken);
    }
}
