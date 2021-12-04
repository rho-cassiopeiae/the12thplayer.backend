using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Application.Livescore.Commands.ActivateFixture {
    public class ActivateFixtureCommand : IRequest<VoidResult> {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
    }

    public class ActivateFixtureCommandHandler : IRequestHandler<
        ActivateFixtureCommand, VoidResult
    > {
        private readonly IInMemoryStore _inMemoryStore;

        public ActivateFixtureCommandHandler(IInMemoryStore inMemoryStore) {
            _inMemoryStore = inMemoryStore;
        }

        public async Task<VoidResult> Handle(
            ActivateFixtureCommand command, CancellationToken cancellationToken
        ) {
            _inMemoryStore.SetFixtureActiveAndOngoing(command.FixtureId, command.TeamId);
            _inMemoryStore.CreateDiscussionsFor(command.FixtureId, command.TeamId);

            await _inMemoryStore.SaveChanges();

            return VoidResult.Instance;
        }
    }
}
