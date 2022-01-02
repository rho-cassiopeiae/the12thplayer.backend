using System.Collections.Generic;
using System.Threading;

using Livescore.Application.Common.Dto;

namespace Livescore.Infrastructure.InMemory.Listeners.FixtureDiscussionListener {
    public interface IFixtureDiscussionListener {
        IAsyncEnumerable<FixtureDiscussionUpdateDto> ListenForDiscussionUpdates(CancellationToken stoppingToken);
    }
}
