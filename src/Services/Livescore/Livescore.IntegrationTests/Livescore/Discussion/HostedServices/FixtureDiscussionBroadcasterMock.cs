using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Api.Services.FixtureDiscussionBroadcaster;
using Livescore.Infrastructure.InMemory.Listeners.FixtureDiscussionListener;

namespace Livescore.IntegrationTests.Livescore.Discussion.HostedServices {
    public class FixtureDiscussionBroadcasterMock : IFixtureDiscussionBroadcaster {
        private List<FixtureDiscussionUpdateDto> _updates = new();
        public IReadOnlyList<FixtureDiscussionUpdateDto> Updates => _updates;

        public void Reset() {
            _updates.Clear();
        }

        public Task BroadcastDiscussionUpdate(FixtureDiscussionUpdateDto fixtureDiscussionUpdate) {
            _updates.Add(fixtureDiscussionUpdate);

            return Task.CompletedTask;
        }
    }
}
