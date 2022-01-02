using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using Livescore.Api.Hubs;
using Livescore.Api.Hubs.Clients;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Api.Services {
    public class FixtureLivescoreBroadcaster : IFixtureLivescoreBroadcaster {
        private readonly ILogger<FixtureLivescoreBroadcaster> _logger;
        private readonly IHubContext<FanzoneHub, ILivescoreClient> _hub;

        public FixtureLivescoreBroadcaster(
            ILogger<FixtureLivescoreBroadcaster> logger,
            IHubContext<FanzoneHub, ILivescoreClient> hub
        ) {
            _logger = logger;
            _hub = hub;
        }

        public Task SubscribeToFixture(string connectionId, long fixtureId, long teamId) {
            _logger.LogInformation(
                "Client {ConnectionId} SUBSCRIBED to Fixture {FixtureId} Team {TeamId}",
                connectionId, fixtureId, teamId
            );

            return _hub.Groups.AddToGroupAsync(connectionId, $"f:{fixtureId}.t:{teamId}");
        }

        //public Task SubscribeToFixture(string connectionId, string fixtureIdentifier) {
        //    _logger.LogInformation(
        //        "Client {ConnectionId} SUBSCRIBED to fixture {FixtureIdentifier}",
        //        connectionId, fixtureIdentifier
        //    );

        //    return _hub.Groups.AddToGroupAsync(connectionId, fixtureIdentifier);
        //}

        public Task UnsubscribeFromFixture(string connectionId, long fixtureId, long teamId) {
            _logger.LogInformation(
                "Client {ConnectionId} UNSUBSCRIBED from Fixture {FixtureId} Team {TeamId}",
                connectionId, fixtureId, teamId
            );

            return _hub.Groups.RemoveFromGroupAsync(connectionId, $"f:{fixtureId}.t:{teamId}");
        }

        public Task BroadcastUpdate(long fixtureId, long teamId, string update) =>
            _hub.Clients.Group($"f:{fixtureId}.t:{teamId}").UpdateFixtureLivescore(update);
    }
}
