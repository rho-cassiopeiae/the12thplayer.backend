using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;

namespace Livescore.Application.Livescore.Fixture.Commands.UnsubscribeFromFixture {
    public class UnsubscribeFromFixtureCommand : IRequest<VoidResult> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
    }

    public class UnsubscribeFromFixtureCommandHandler : IRequestHandler<UnsubscribeFromFixtureCommand, VoidResult> {
        private readonly IConnectionIdProvider _connectionIdProvider;
        private readonly IFixtureLivescoreBroadcaster _fixtureLivescoreBroadcaster;

        public UnsubscribeFromFixtureCommandHandler(
            IConnectionIdProvider connectionIdProvider,
            IFixtureLivescoreBroadcaster fixtureLivescoreBroadcaster
        ) {
            _connectionIdProvider = connectionIdProvider;
            _fixtureLivescoreBroadcaster = fixtureLivescoreBroadcaster;
        }

        public async Task<VoidResult> Handle(
            UnsubscribeFromFixtureCommand command, CancellationToken cancellationToken
        ) {
            await _fixtureLivescoreBroadcaster.UnsubscribeFromFixture(
                _connectionIdProvider.ConnectionId, command.FixtureId, command.TeamId
            );

            return VoidResult.Instance;
        }
    }
}
