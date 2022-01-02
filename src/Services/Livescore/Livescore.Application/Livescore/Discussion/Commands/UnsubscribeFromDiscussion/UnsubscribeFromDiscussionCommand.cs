using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;

namespace Livescore.Application.Livescore.Discussion.Commands.UnsubscribeFromDiscussion {
    public class UnsubscribeFromDiscussionCommand : IRequest<VoidResult> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public string DiscussionId { get; set; }
    }

    public class UnsubscribeFromDiscussionCommandHandler : IRequestHandler<UnsubscribeFromDiscussionCommand, VoidResult> {
        private readonly IConnectionIdProvider _connectionIdProvider;
        private readonly IFixtureDiscussionBroadcaster _fixtureDiscussionBroadcaster;

        public UnsubscribeFromDiscussionCommandHandler(
            IConnectionIdProvider connectionIdProvider,
            IFixtureDiscussionBroadcaster fixtureDiscussionBroadcaster
        ) {
            _connectionIdProvider = connectionIdProvider;
            _fixtureDiscussionBroadcaster = fixtureDiscussionBroadcaster;
        }

        public async Task<VoidResult> Handle(
            UnsubscribeFromDiscussionCommand command, CancellationToken cancellationToken
        ) {
            await _fixtureDiscussionBroadcaster.UnsubscribeFromDiscussion(
                _connectionIdProvider.ConnectionId, command.FixtureId, command.TeamId, command.DiscussionId
            );

            return VoidResult.Instance;
        }
    }
}
