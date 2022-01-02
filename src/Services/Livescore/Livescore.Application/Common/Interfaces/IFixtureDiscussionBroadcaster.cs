using System.Threading.Tasks;

using Livescore.Application.Common.Dto;

namespace Livescore.Application.Common.Interfaces {
    public interface IFixtureDiscussionBroadcaster {
        Task SubscribeToDiscussion(string connectionId, long fixtureId, long teamId, string discussionId);
        Task UnsubscribeFromDiscussion(string connectionId, long fixtureId, long teamId, string discussionId);
        Task BroadcastUpdate(FixtureDiscussionUpdateDto update);
    }
}
