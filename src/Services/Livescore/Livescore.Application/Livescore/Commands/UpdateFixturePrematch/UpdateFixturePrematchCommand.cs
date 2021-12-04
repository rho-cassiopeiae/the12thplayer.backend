using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Dto;
using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.Fixture;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Application.Livescore.Commands.UpdateFixturePrematch {
    public class UpdateFixturePrematchCommand : IRequest<VoidResult> {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
        public FixtureDto Fixture { get; init; }
    }

    public class UpdateFixturePrematchCommandHandler : IRequestHandler<
        UpdateFixturePrematchCommand, VoidResult
    > {
        private readonly IFixtureRepository _fixtureRepository;
        private readonly IInMemoryStore _inMemoryStore;

        public UpdateFixturePrematchCommandHandler(
            IFixtureRepository fixtureRepository,
            IInMemoryStore inMemoryStore
        ) {
            _fixtureRepository = fixtureRepository;
            _inMemoryStore = inMemoryStore;
        }

        public async Task<VoidResult> Handle(
            UpdateFixturePrematchCommand command, CancellationToken cancellationToken
        ) {
            var trackedFixture = command.Fixture;

            var fixture = await _fixtureRepository.FindByKey(command.FixtureId, command.TeamId);

            fixture.SetStartTime(
                new DateTimeOffset(trackedFixture.StartTime.Value).ToUnixTimeMilliseconds()
            );
            fixture.SetStatus(trackedFixture.Status);
            fixture.SetReferee(trackedFixture.RefereeName);

            fixture.SetColors(new[] {
                new TeamColor(
                    teamId: command.TeamId,
                    color: trackedFixture.Colors.First(c => c.TeamId == command.TeamId).Color
                ),
                new TeamColor(
                    teamId: trackedFixture.OpponentTeam.Id,
                    color: trackedFixture.Colors.First(c => c.TeamId == trackedFixture.OpponentTeam.Id).Color
                )
            });

            var teamLineup = trackedFixture.Lineups.First(l => l.TeamId == command.TeamId);
            var opponentTeamLineup = trackedFixture.Lineups.First(l => l.TeamId == trackedFixture.OpponentTeam.Id);

            fixture.SetLineups(new[] {
                new TeamLineup(
                    teamId: command.TeamId,
                    formation: teamLineup.Formation,
                    manager: teamLineup.Manager != null ?
                        new TeamLineup._Manager(
                            id: teamLineup.Manager.Id,
                            name: teamLineup.Manager.Name,
                            imageUrl: teamLineup.Manager.ImageUrl
                        ) :
                        null,
                    startingXI: teamLineup.StartingXI?.Select(p => new TeamLineup.Player(
                        id: p.Id,
                        firstName: p.FirstName,
                        lastName: p.LastName,
                        number: p.Number,
                        isCaptain: p.IsCaptain,
                        position: p.Position,
                        formationPosition: p.FormationPosition,
                        imageUrl: p.ImageUrl,
                        rating: p.Rating
                    )),
                    subs: teamLineup.Subs?.Select(p => new TeamLineup.Player(
                        id: p.Id,
                        firstName: p.FirstName,
                        lastName: p.LastName,
                        number: p.Number,
                        isCaptain: p.IsCaptain,
                        position: p.Position,
                        formationPosition: p.FormationPosition,
                        imageUrl: p.ImageUrl,
                        rating: p.Rating
                    ))
                ),
                new TeamLineup(
                    teamId: trackedFixture.OpponentTeam.Id,
                    formation: opponentTeamLineup.Formation,
                    manager: opponentTeamLineup.Manager != null ?
                        new TeamLineup._Manager(
                            id: opponentTeamLineup.Manager.Id,
                            name: opponentTeamLineup.Manager.Name,
                            imageUrl: opponentTeamLineup.Manager.ImageUrl
                        ) :
                        null,
                    startingXI: opponentTeamLineup.StartingXI?.Select(p => new TeamLineup.Player(
                        id: p.Id,
                        firstName: p.FirstName,
                        lastName: p.LastName,
                        number: p.Number,
                        isCaptain: p.IsCaptain,
                        position: p.Position,
                        formationPosition: p.FormationPosition,
                        imageUrl: p.ImageUrl,
                        rating: p.Rating
                    )),
                    subs: opponentTeamLineup.Subs?.Select(p => new TeamLineup.Player(
                        id: p.Id,
                        firstName: p.FirstName,
                        lastName: p.LastName,
                        number: p.Number,
                        isCaptain: p.IsCaptain,
                        position: p.Position,
                        formationPosition: p.FormationPosition,
                        imageUrl: p.ImageUrl,
                        rating: p.Rating
                    ))
                )
            });

            await _fixtureRepository.SaveChanges(cancellationToken);

            _inMemoryStore.AddFixtureParticipantsFromLineup(command.FixtureId, command.TeamId, teamLineup);

            await _inMemoryStore.SaveChanges();

            return VoidResult.Instance;
        }
    }
}
