using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using Livescore.Api.Hubs;
using Livescore.Api.Hubs.Clients;
using Livescore.Application.Common.Dto;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Api.Services {
    public class FixtureDiscussionBroadcaster : IFixtureDiscussionBroadcaster {
        private readonly ILogger<FixtureDiscussionBroadcaster> _logger;
        private readonly IHubContext<FanzoneHub, ILivescoreClient> _hub;

        public FixtureDiscussionBroadcaster(
            ILogger<FixtureDiscussionBroadcaster> logger,
            IHubContext<FanzoneHub, ILivescoreClient> hub
        ) {
            _logger = logger;
            _hub = hub;
        }

        public Task SubscribeToDiscussion(string connectionId, long fixtureId, long teamId, string discussionId) {
            _logger.LogInformation(
                "Client {ConnectionId} SUBSCRIBED to Fixture {FixtureId} Team {TeamId} Discussion {DiscussionId}",
                connectionId, fixtureId, teamId, discussionId
            );

            return _hub.Groups.AddToGroupAsync(connectionId, $"f:{fixtureId}.t:{teamId}.d:{discussionId}");
        }

        //public Task SubscribeToDiscussion(string connectionId, string discussionFullIdentifier) {
        //    logger.LogInformation(
        //        "Client {ConnectionId} SUBSCRIBED to discussion {DiscussionFullIdentifier}",
        //        connectionId, discussionFullIdentifier
        //    );

        //    return hub.Groups.AddToGroupAsync(connectionId, discussionFullIdentifier);
        //}

        public Task UnsubscribeFromDiscussion(string connectionId, long fixtureId, long teamId, string discussionId) {
            _logger.LogInformation(
                "Client {ConnectionId} UNSUBSCRIBED from Fixture {FixtureId} Team {TeamId} Discussion {DiscussionId}",
                connectionId, fixtureId, teamId, discussionId
            );

            return _hub.Groups.RemoveFromGroupAsync(connectionId, $"f:{fixtureId}.t:{teamId}.d:{discussionId}");
        }

        public Task BroadcastUpdate(FixtureDiscussionUpdateDto update) {
            return _hub.Clients
                .Group($"f:{update.FixtureId}.t:{update.TeamId}.d:{update.DiscussionId}")
                .UpdateFixtureDiscussion(update);
        }
    }
}
