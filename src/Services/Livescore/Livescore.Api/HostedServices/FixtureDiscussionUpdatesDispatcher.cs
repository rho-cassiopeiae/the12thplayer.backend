using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using Livescore.Api.Services.FixtureDiscussionBroadcaster;
using Livescore.Infrastructure.InMemory.Listeners.FixtureDiscussionListener;

namespace Livescore.Api.HostedServices {
    public class FixtureDiscussionUpdatesDispatcher : BackgroundService {
        private readonly IFixtureDiscussionListener _fixtureDiscussionListener;
        private readonly IFixtureDiscussionBroadcaster _fixtureDiscussionBroadcaster;

        public FixtureDiscussionUpdatesDispatcher(
            IFixtureDiscussionListener fixtureDiscussionListener,
            IFixtureDiscussionBroadcaster fixtureDiscussionBroadcaster
        ) {
            _fixtureDiscussionListener = fixtureDiscussionListener;
            _fixtureDiscussionBroadcaster = fixtureDiscussionBroadcaster;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await foreach (var fixtureDiscussionUpdate in
                _fixtureDiscussionListener.ListenForDiscussionUpdates(stoppingToken)
            ) {
                await _fixtureDiscussionBroadcaster.BroadcastDiscussionUpdate(fixtureDiscussionUpdate);
            }
        }
    }
}
