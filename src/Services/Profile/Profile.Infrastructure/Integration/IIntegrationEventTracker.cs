using System;
using System.Threading;
using System.Threading.Tasks;

namespace Profile.Infrastructure.Integration {
    public interface IIntegrationEventTracker : IAsyncDisposable {
        Task ListenForAndPublishNewEvents(CancellationToken stoppingToken);
    }
}
