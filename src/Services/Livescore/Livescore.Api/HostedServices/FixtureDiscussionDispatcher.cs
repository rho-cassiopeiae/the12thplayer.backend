using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using Livescore.Infrastructure.InMemory.Listeners.FixtureDiscussionListener;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Api.HostedServices {
    public class FixtureDiscussionDispatcher : BackgroundService {
        private readonly IFixtureDiscussionListener _fixtureDiscussionListener;
        private readonly IFixtureDiscussionBroadcaster _fixtureDiscussionBroadcaster;

        public FixtureDiscussionDispatcher(
            IFixtureDiscussionListener fixtureDiscussionListener,
            IFixtureDiscussionBroadcaster fixtureDiscussionBroadcaster
        ) {
            _fixtureDiscussionListener = fixtureDiscussionListener;
            _fixtureDiscussionBroadcaster = fixtureDiscussionBroadcaster;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            // @@TODO: Make discussion listener restartable.
            await foreach (
                var update in _fixtureDiscussionListener.ListenForDiscussionUpdates(stoppingToken)
            ) {
                await _fixtureDiscussionBroadcaster.BroadcastUpdate(update);
            }
        }
    }
}
