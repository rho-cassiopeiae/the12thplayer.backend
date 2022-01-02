using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.FixtureLivescoreStatus;

namespace Livescore.Application.Livescore.Worker.Commands.FinishFixture {
    public class FinishFixtureCommand : IRequest<VoidResult> {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
    }

    public class FinishFixtureCommandHandler : IRequestHandler<FinishFixtureCommand, VoidResult> {
        private readonly IFixtureLivescoreStatusInMemRepository _fixtureLivescoreStatusInMemRepository;

        public FinishFixtureCommandHandler(
            IFixtureLivescoreStatusInMemRepository fixtureLivescoreStatusInMemRepository
        ) {
            _fixtureLivescoreStatusInMemRepository = fixtureLivescoreStatusInMemRepository;
        }

        public async Task<VoidResult> Handle(
            FinishFixtureCommand command, CancellationToken cancellationToken
        ) {
            var fixtureStatus = new FixtureLivescoreStatus(
                fixtureId: command.FixtureId,
                teamId: command.TeamId,
                active: null,
                ongoing: false
            );

            _fixtureLivescoreStatusInMemRepository.SetOrUpdate(fixtureStatus);
            await _fixtureLivescoreStatusInMemRepository.SaveChanges();

            return VoidResult.Instance;
        }
    }
}
