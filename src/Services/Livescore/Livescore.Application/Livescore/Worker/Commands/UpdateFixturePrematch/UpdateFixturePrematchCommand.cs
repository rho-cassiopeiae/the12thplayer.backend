using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Dto;
using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.Fixture;
using Livescore.Domain.Aggregates.PlayerRating;

namespace Livescore.Application.Livescore.Worker.Commands.UpdateFixturePrematch {
    public class UpdateFixturePrematchCommand : IRequest<VoidResult> {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
        public FixtureDto Fixture { get; init; }
    }

    public class UpdateFixturePrematchCommandHandler : IRequestHandler<
        UpdateFixturePrematchCommand, VoidResult
    > {
        private readonly IFixtureRepository _fixtureRepository;
        private readonly IPlayerRatingInMemRepository _playerRatingInMemRepository;

        public UpdateFixturePrematchCommandHandler(
            IFixtureRepository fixtureRepository,
            IPlayerRatingInMemRepository playerRatingInMemRepository
        ) {
            _fixtureRepository = fixtureRepository;
            _playerRatingInMemRepository = playerRatingInMemRepository;
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

            if (teamLineup.Manager != null) {
                _playerRatingInMemRepository.CreateIfNotExists(new PlayerRating(
                    fixtureId: command.FixtureId,
                    teamId: command.TeamId,
                    participantKey: $"m:{teamLineup.Manager.Id}",
                    totalRating: 0,
                    totalVoters: 0
                ));
            }

            if (teamLineup.StartingXI != null) {
                foreach (var player in teamLineup.StartingXI) {
                    _playerRatingInMemRepository.CreateIfNotExists(new PlayerRating(
                        fixtureId: command.FixtureId,
                        teamId: command.TeamId,
                        participantKey: $"p:{player.Id}",
                        totalRating: 0,
                        totalVoters: 0
                    ));
                }
            }

            await _playerRatingInMemRepository.SaveChanges();

            return VoidResult.Instance;
        }
    }
}
