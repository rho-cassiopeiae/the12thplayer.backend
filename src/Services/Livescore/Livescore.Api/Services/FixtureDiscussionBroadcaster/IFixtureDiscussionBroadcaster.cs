using System.Threading.Tasks;

using Livescore.Infrastructure.InMemory.Listeners.FixtureDiscussionListener;

namespace Livescore.Api.Services.FixtureDiscussionBroadcaster {
    public interface IFixtureDiscussionBroadcaster {
        Task BroadcastDiscussionUpdate(FixtureDiscussionUpdateDto fixtureDiscussionUpdate);
    }
}
