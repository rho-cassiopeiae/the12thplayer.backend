using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Application.Common.Dto;
using Livescore.Application.Common.Interfaces;

namespace Livescore.IntegrationTests.Livescore.Discussion.Mocks {
    public class FixtureDiscussionBroadcasterMock : IFixtureDiscussionBroadcaster {
        private List<FixtureDiscussionUpdateDto> _updates = new();
        public IReadOnlyList<FixtureDiscussionUpdateDto> Updates => _updates;

        public void Reset() {
            _updates.Clear();
        }

        public Task SubscribeToDiscussion(string connectionId, long fixtureId, long teamId, string discussionId) {
            throw new System.NotImplementedException();
        }

        public Task UnsubscribeFromDiscussion(string connectionId, long fixtureId, long teamId, string discussionId) {
            throw new System.NotImplementedException();
        }

        public Task BroadcastUpdate(FixtureDiscussionUpdateDto update) {
            _updates.Add(update);

            return Task.CompletedTask;
        }
    }
}
