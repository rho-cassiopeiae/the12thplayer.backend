using System.Threading.Tasks;

namespace Livescore.Application.Common.Interfaces {
    public interface IFixtureLivescoreBroadcaster {
        Task SubscribeToFixture(string connectionId, long fixtureId, long teamId);

        //public Task SubscribeToFixture(string connectionId, string fixtureIdentifier);

        Task UnsubscribeFromFixture(string connectionId, long fixtureId, long teamId);

        Task BroadcastUpdate(long fixtureId, long teamId, string update);
    }
}
