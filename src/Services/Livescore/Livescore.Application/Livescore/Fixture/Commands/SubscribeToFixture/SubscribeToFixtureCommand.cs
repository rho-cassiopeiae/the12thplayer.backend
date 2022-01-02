using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;

namespace Livescore.Application.Livescore.Fixture.Commands.SubscribeToFixture {
    public class SubscribeToFixtureCommand : IRequest<VoidResult> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
    }

    public class SubscribeToFixtureCommandHandler : IRequestHandler<SubscribeToFixtureCommand, VoidResult> {
        private readonly IConnectionIdProvider _connectionIdProvider;
        private readonly IFixtureLivescoreBroadcaster _fixtureLivescoreBroadcaster;

        public SubscribeToFixtureCommandHandler(
            IConnectionIdProvider connectionIdProvider,
            IFixtureLivescoreBroadcaster fixtureLivescoreBroadcaster
        ) {
            _connectionIdProvider = connectionIdProvider;
            _fixtureLivescoreBroadcaster = fixtureLivescoreBroadcaster;
        }

        public async Task<VoidResult> Handle(
            SubscribeToFixtureCommand command, CancellationToken cancellationToken
        ) {
            await _fixtureLivescoreBroadcaster.SubscribeToFixture(
                _connectionIdProvider.ConnectionId, command.FixtureId, command.TeamId
            );

            return VoidResult.Instance;
        }
    }
}
