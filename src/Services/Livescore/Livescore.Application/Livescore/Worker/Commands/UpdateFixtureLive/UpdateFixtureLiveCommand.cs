using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Dto;
using Livescore.Application.Common.Results;
using Livescore.Application.Common.Interfaces;
using Livescore.Application.Livescore.Worker.Common.Dto;
using Livescore.Domain.Aggregates.Fixture;
using Livescore.Domain.Aggregates.PlayerRating;
using PlayerRatingDm = Livescore.Domain.Aggregates.PlayerRating.PlayerRating;

namespace Livescore.Application.Livescore.Worker.Commands.UpdateFixtureLive {
    public class UpdateFixtureLiveCommand : IRequest<VoidResult> {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
        public FixtureDto Fixture { get; init; }
    }

    public class UpdateFixtureLiveCommandHandler : IRequestHandler<
        UpdateFixtureLiveCommand, VoidResult
    > {
        private readonly IFixtureRepository _fixtureRepository;
        private readonly IPlayerRatingInMemRepository _playerRatingInMemRepository;

        private readonly ISerializer _serializer;
        private readonly IFixtureLivescoreBroadcaster _fixtureLivescoreBroadcaster;

        public UpdateFixtureLiveCommandHandler(
            IFixtureRepository fixtureRepository,
            IPlayerRatingInMemRepository playerRatingInMemRepository,
            ISerializer serializer,
            IFixtureLivescoreBroadcaster fixtureLivescoreBroadcaster
        ) {
            _fixtureRepository = fixtureRepository;
            _playerRatingInMemRepository = playerRatingInMemRepository;
            _serializer = serializer;
            _fixtureLivescoreBroadcaster = fixtureLivescoreBroadcaster;
        }

        public async Task<VoidResult> Handle(
            UpdateFixtureLiveCommand command, CancellationToken cancellationToken
        ) {
            var trackedFixture = command.Fixture;

            var fixture = await _fixtureRepository.FindByKey(command.FixtureId, command.TeamId);

            fixture.SetStatus(trackedFixture.Status);
            
            fixture.SetGameTime(new GameTime(
                minute: trackedFixture.GameTime.Minute,
                extraTimeMinute: trackedFixture.GameTime.ExtraTimeMinute,
                addedTimeMinute: trackedFixture.GameTime.AddedTimeMinute
            ));

            fixture.SetScore(new Score(
                localTeam: trackedFixture.Score.LocalTeam,
                visitorTeam: trackedFixture.Score.VisitorTeam,
                ht: trackedFixture.Score.HT,
                ft: trackedFixture.Score.FT,
                et: trackedFixture.Score.ET,
                ps: trackedFixture.Score.PS
            ));

            var teamMatchEvents = trackedFixture.Events.First(e => e.TeamId == command.TeamId);
            var opponentTeamMatchEvents = trackedFixture.Events.First(e => e.TeamId == trackedFixture.OpponentTeam.Id);

            var teamStats = trackedFixture.Stats.First(s => s.TeamId == command.TeamId);
            var opponentTeamStats = trackedFixture.Stats.First(s => s.TeamId == trackedFixture.OpponentTeam.Id);

            fixture.SetEvents(new[] {
                new TeamMatchEvents(
                    teamId: command.TeamId,
                    events: teamMatchEvents.Events.Select(e => new TeamMatchEvents.MatchEvent(
                        minute: e.Minute,
                        addedTimeMinute: e.AddedTimeMinute,
                        type: e.Type,
                        playerId: e.PlayerId,
                        relatedPlayerId: e.RelatedPlayerId
                    ))
                ),
                new TeamMatchEvents(
                    teamId: trackedFixture.OpponentTeam.Id,
                    events: opponentTeamMatchEvents.Events.Select(e => new TeamMatchEvents.MatchEvent(
                        minute: e.Minute,
                        addedTimeMinute: e.AddedTimeMinute,
                        type: e.Type,
                        playerId: e.PlayerId,
                        relatedPlayerId: e.RelatedPlayerId
                    ))
                )
            });

            fixture.SetStats(new[] {
                new TeamStats(
                    teamId: command.TeamId,
                    stats: teamStats.Stats != null ?
                        new TeamStats._Stats(
                            shots: teamStats.Stats.Shots != null ?
                                new TeamStats._Stats.ShotStats(
                                    total: teamStats.Stats.Shots.Total,
                                    onTarget: teamStats.Stats.Shots.OnTarget,
                                    offTarget: teamStats.Stats.Shots.OffTarget,
                                    blocked: teamStats.Stats.Shots.Blocked,
                                    insideBox: teamStats.Stats.Shots.InsideBox,
                                    outsideBox: teamStats.Stats.Shots.OutsideBox
                                ) :
                                null,
                            passes: teamStats.Stats.Passes != null ?
                                new TeamStats._Stats.PassStats(
                                    total: teamStats.Stats.Passes.Total,
                                    accurate: teamStats.Stats.Passes.Accurate
                                ) :
                                null,
                            fouls: teamStats.Stats.Fouls,
                            corners: teamStats.Stats.Corners,
                            offsides: teamStats.Stats.Offsides,
                            ballPossession: teamStats.Stats.BallPossession,
                            yellowCards: teamStats.Stats.YellowCards,
                            redCards: teamStats.Stats.RedCards,
                            tackles: teamStats.Stats.Tackles
                        ) :
                        null
                ),
                new TeamStats(
                    teamId: trackedFixture.OpponentTeam.Id,
                    stats: opponentTeamStats.Stats != null ?
                        new TeamStats._Stats(
                            shots: opponentTeamStats.Stats.Shots != null ?
                                new TeamStats._Stats.ShotStats(
                                    total: opponentTeamStats.Stats.Shots.Total,
                                    onTarget: opponentTeamStats.Stats.Shots.OnTarget,
                                    offTarget: opponentTeamStats.Stats.Shots.OffTarget,
                                    blocked: opponentTeamStats.Stats.Shots.Blocked,
                                    insideBox: opponentTeamStats.Stats.Shots.InsideBox,
                                    outsideBox: opponentTeamStats.Stats.Shots.OutsideBox
                                ) :
                                null,
                            passes: opponentTeamStats.Stats.Passes != null ?
                                new TeamStats._Stats.PassStats(
                                    total: opponentTeamStats.Stats.Passes.Total,
                                    accurate: opponentTeamStats.Stats.Passes.Accurate
                                ) :
                                null,
                            fouls: opponentTeamStats.Stats.Fouls,
                            corners: opponentTeamStats.Stats.Corners,
                            offsides: opponentTeamStats.Stats.Offsides,
                            ballPossession: opponentTeamStats.Stats.BallPossession,
                            yellowCards: opponentTeamStats.Stats.YellowCards,
                            redCards: opponentTeamStats.Stats.RedCards,
                            tackles: opponentTeamStats.Stats.Tackles
                        ) :
                        null
                )
            });

            await _fixtureRepository.SaveChanges(cancellationToken);

            var subs = teamMatchEvents.Events.Where(e =>
                e.Type.ToLowerInvariant() == "substitution" &&
                e.PlayerId != null
            );
            foreach (var sub in subs) {
                _playerRatingInMemRepository.CreateIfNotExists(new PlayerRatingDm(
                    fixtureId: command.FixtureId,
                    teamId: command.TeamId,
                    participantKey: $"s:{sub.PlayerId.Value}",
                    totalRating: 0,
                    totalVoters: 0
                ));
            }

            await _playerRatingInMemRepository.SaveChanges();

            var fixtureLivescoreUpdate = new FixtureLivescoreUpdateDto {
                FixtureId = command.FixtureId,
                TeamId = command.TeamId,
                Status = trackedFixture.Status,
                GameTime = trackedFixture.GameTime,
                Score = trackedFixture.Score,
                Events = trackedFixture.Events,
                Stats = trackedFixture.Stats
            };

            string updateMessage = _serializer.Serialize(fixtureLivescoreUpdate);

            await _fixtureLivescoreBroadcaster.BroadcastUpdate(command.FixtureId, command.TeamId, updateMessage);

            return VoidResult.Instance;
        }
    }
}
