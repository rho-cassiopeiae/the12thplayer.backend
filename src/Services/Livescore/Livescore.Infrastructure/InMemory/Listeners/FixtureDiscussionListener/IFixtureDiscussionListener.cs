using System.Collections.Generic;
using System.Threading;

namespace Livescore.Infrastructure.InMemory.Listeners.FixtureDiscussionListener {
    public interface IFixtureDiscussionListener {
        IAsyncEnumerable<FixtureDiscussionUpdateDto> ListenForDiscussionUpdates(CancellationToken stoppingToken);
    }
}
