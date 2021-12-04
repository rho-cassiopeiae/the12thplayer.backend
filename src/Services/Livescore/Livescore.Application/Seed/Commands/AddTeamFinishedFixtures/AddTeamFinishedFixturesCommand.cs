﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Application.Common.Dto;
using Livescore.Domain.Aggregates.Team;
using Livescore.Domain.Aggregates.Venue;
using Livescore.Domain.Aggregates.League;
using Livescore.Domain.Aggregates.Player;
using Livescore.Domain.Aggregates.Fixture;

namespace Livescore.Application.Seed.Commands.AddTeamFinishedFixtures {
    public class AddTeamFinishedFixturesCommand : IRequest<VoidResult> {
        public long TeamId { get; init; }
        public IEnumerable<FixtureDto> Fixtures { get; init; }
        public IEnumerable<SeasonDto> Seasons { get; init; }
        public IEnumerable<PlayerDto> Players { get; init; }
    }

    public class AddTeamFinishedFixturesCommandHandler : IRequestHandler<
        AddTeamFinishedFixturesCommand, VoidResult
    > {
        private readonly ITeamRepository _teamRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly ILeagueRepository _leagueRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IFixtureRepository _fixtureRepository;

        public AddTeamFinishedFixturesCommandHandler(
            ITeamRepository teamRepository,
            IVenueRepository venueRepository,
            ILeagueRepository leagueRepository,
            IPlayerRepository playerRepository,
            IFixtureRepository fixtureRepository
        ) {
            _teamRepository = teamRepository;
            _venueRepository = venueRepository;
            _leagueRepository = leagueRepository;
            _playerRepository = playerRepository;
            _fixtureRepository = fixtureRepository;
        }

        public async Task<VoidResult> Handle(
            AddTeamFinishedFixturesCommand command, CancellationToken cancellationToken
        ) {
            long teamId = command.TeamId;

            var opponentTeams = command.Fixtures
                .Select(f => f.OpponentTeam)
                .Distinct()
                .ToList();

            var teams = await _teamRepository.FindById(opponentTeams.Select(t => t.Id));

            foreach (var opponentTeam in opponentTeams) {
                if (!teams.Any(t => t.Id == opponentTeam.Id)) {
                    _teamRepository.Create(new Team(
                        id: opponentTeam.Id,
                        name: opponentTeam.Name,
                        countryId: opponentTeam.CountryId,
                        logoUrl: opponentTeam.LogoUrl,
                        hasThe12thPlayerCommunity: false
                    ));
                }
            }

            var fixtureVenues = command.Fixtures
                .Where(fixture => fixture.Venue != null)
                .Select(fixture => fixture.Venue)
                .Distinct()
                .ToList();

            var venues = await _venueRepository.FindById(fixtureVenues.Select(v => v.Id));

            foreach (var fixtureVenue in fixtureVenues) {
                if (!venues.Any(v => v.Id == fixtureVenue.Id)) {
                    _venueRepository.Create(new Venue(
                        id: fixtureVenue.Id,
                        teamId: fixtureVenue.TeamId,
                        name: fixtureVenue.Name,
                        city: fixtureVenue.City,
                        capacity: fixtureVenue.Capacity,
                        imageUrl: fixtureVenue.ImageUrl
                    ));
                }
            }

            var fixtureLeagues = command.Seasons
                .Select(s => s.League)
                .Distinct()
                .ToList();

            var leagues = await _leagueRepository.FindById(fixtureLeagues.Select(l => l.Id));

            foreach (var fixtureLeague in fixtureLeagues) {
                var league = leagues.FirstOrDefault(l => l.Id == fixtureLeague.Id);
                if (league == null) {
                    league = new League(
                        id: fixtureLeague.Id,
                        name: fixtureLeague.Name,
                        type: fixtureLeague.Type,
                        isCup: fixtureLeague.IsCup,
                        logoUrl: fixtureLeague.LogoUrl
                    );

                    foreach (
                        var fixtureSeason in command.Seasons.Where(
                            s => s.League.Id == league.Id
                        )
                    ) {
                        league.AddSeason(new Season(
                            id: fixtureSeason.Id,
                            name: fixtureSeason.Name,
                            isCurrent: false // @@NOTE: Have to set it manually from admin panel later.
                        ));
                    }

                    _leagueRepository.Create(league);
                } else {
                    foreach (
                        var fixtureSeason in command.Seasons.Where(
                            s => s.League.Id == league.Id
                        )
                    ) {
                        if (!league.Seasons.Any(s => s.Id == fixtureSeason.Id)) {
                            // @@TODO: Check that EF Core detects this season addition.
                            // Do we need to track league for this or AsNoTracking is fine? Most likely have to track.
                            league.AddSeason(new Season(
                                id: fixtureSeason.Id,
                                name: fixtureSeason.Name,
                                isCurrent: false
                            ));
                        }
                    }
                }
            }

            var fixturePlayers = command.Players;

            var players = await _playerRepository.FindById(fixturePlayers.Select(p => p.Id));

            foreach (var fixturePlayer in fixturePlayers) {
                var player = players.FirstOrDefault(p => p.Id == fixturePlayer.Id);
                if (player == null) {
                    _playerRepository.Create(new Player(
                        id: fixturePlayer.Id,
                        teamId: teamId,
                        firstName: fixturePlayer.FirstName,
                        lastName: fixturePlayer.LastName,
                        birthDate: fixturePlayer.BirthDate != null ?
                            new DateTimeOffset(fixturePlayer.BirthDate.Value)
                                .ToUnixTimeMilliseconds() :
                            null,
                        countryId: fixturePlayer.CountryId,
                        number: fixturePlayer.Number,
                        position: fixturePlayer.Position,
                        imageUrl: fixturePlayer.ImageUrl,
                        lastLineupAt: new DateTimeOffset(fixturePlayer.LastLineupAt)
                            .ToUnixTimeMilliseconds()
                    ));
                } else {
                    long lastLineupAt = new DateTimeOffset(fixturePlayer.LastLineupAt)
                        .ToUnixTimeMilliseconds();
                    if (player.TeamId != teamId && player.LastLineupAt < lastLineupAt) {
                        player.ChangeTeam(teamId);
                        player.ChangeNumber(fixturePlayer.Number);
                        player.SetPosition(fixturePlayer.Position);
                        player.SetLastLineupAt(lastLineupAt);
                    }
                }
            }

            var fixtures = await _fixtureRepository.FindByTeamId(teamId);

            foreach (var fixture in command.Fixtures) {
                if (!fixtures.Any(f => f.Id == fixture.Id)) {
                    var teamLineup = fixture.Lineups.First(l => l.TeamId == teamId);
                    var opponentTeamLineup = fixture.Lineups.First(l => l.TeamId == fixture.OpponentTeam.Id);

                    var teamMatchEvents = fixture.Events.First(e => e.TeamId == teamId);
                    var opponentTeamMatchEvents = fixture.Events.First(e => e.TeamId == fixture.OpponentTeam.Id);

                    var teamStats = fixture.Stats.First(s => s.TeamId == teamId);
                    var opponentTeamStats = fixture.Stats.First(s => s.TeamId == fixture.OpponentTeam.Id);

                    _fixtureRepository.Create(new Fixture(
                        id: fixture.Id,
                        teamId: teamId,
                        seasonId: fixture.SeasonId,
                        opponentTeamId: fixture.OpponentTeam.Id,
                        homeStatus: fixture.HomeStatus,
                        startTime: new DateTimeOffset(fixture.StartTime.Value).ToUnixTimeMilliseconds(),
                        status: fixture.Status,
                        gameTime: new GameTime(
                            minute: fixture.GameTime.Minute,
                            extraTimeMinute: fixture.GameTime.ExtraTimeMinute,
                            addedTimeMinute: fixture.GameTime.AddedTimeMinute
                        ),
                        score: new Score(
                            localTeam: fixture.Score.LocalTeam,
                            visitorTeam: fixture.Score.VisitorTeam,
                            ht: fixture.Score.HT,
                            ft: fixture.Score.FT,
                            et: fixture.Score.ET,
                            ps: fixture.Score.PS
                        ),
                        venueId: fixture.Venue?.Id,
                        refereeName: fixture.RefereeName,
                        colors: new[] {
                            new TeamColor(
                                teamId: teamId,
                                color: fixture.Colors.First(c => c.TeamId == teamId).Color
                            ),
                            new TeamColor(
                                teamId: fixture.OpponentTeam.Id,
                                color: fixture.Colors.First(c => c.TeamId == fixture.OpponentTeam.Id).Color
                            )
                        },
                        lineups: new[] {
                            new TeamLineup(
                                teamId: teamId,
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
                                teamId: fixture.OpponentTeam.Id,
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
                        },
                        events: new[] {
                            new TeamMatchEvents(
                                teamId: teamId,
                                events: teamMatchEvents.Events?.Select(e => new TeamMatchEvents.MatchEvent(
                                    minute: e.Minute,
                                    addedTimeMinute: e.AddedTimeMinute,
                                    type: e.Type,
                                    playerId: e.PlayerId,
                                    relatedPlayerId: e.RelatedPlayerId
                                ))
                            ),
                            new TeamMatchEvents(
                                teamId: fixture.OpponentTeam.Id,
                                events: opponentTeamMatchEvents.Events?.Select(e => new TeamMatchEvents.MatchEvent(
                                    minute: e.Minute,
                                    addedTimeMinute: e.AddedTimeMinute,
                                    type: e.Type,
                                    playerId: e.PlayerId,
                                    relatedPlayerId: e.RelatedPlayerId
                                ))
                            )
                        },
                        stats: new[] {
                            new TeamStats(
                                teamId: teamId,
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
                                teamId: fixture.OpponentTeam.Id,
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
                        }
                    ));
                }
            }

            await _fixtureRepository.SaveChanges(cancellationToken);

            return VoidResult.Instance;
        }
    }
}
