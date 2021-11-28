using System;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Integration {
    public interface IIntegrationEventTracker : IAsyncDisposable {
        Task ListenForAndPublishNewEvents(CancellationToken stoppingToken);
    }
}
