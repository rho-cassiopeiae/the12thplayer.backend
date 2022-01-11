using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using MatchPredictions.Application.Common.Dto;
using MatchPredictions.Application.Common.Results;
using MatchPredictions.Domain.Aggregates.Fixture;
using MatchPredictions.Domain.Aggregates.League;
using MatchPredictions.Domain.Aggregates.Round;
using MatchPredictions.Domain.Aggregates.Team;

namespace MatchPredictions.Application.Seed.Commands.AddSeasonWithRoundsAndFixtures {
    public class AddSeasonWithRoundsAndFixturesCommand : IRequest<VoidResult> {
        public SeasonDto Season { get; init; }
    }

    public class AddSeasonWithRoundsAndFixturesCommandHandler : IRequestHandler<
        AddSeasonWithRoundsAndFixturesCommand, VoidResult
    > {
        private readonly ILeagueRepository _leagueRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IFixtureRepository _fixtureRepository;

        public AddSeasonWithRoundsAndFixturesCommandHandler(
            ILeagueRepository leagueRepository,
            IRoundRepository roundRepository,
            ITeamRepository teamRepository,
            IFixtureRepository fixtureRepository
        ) {
            _leagueRepository = leagueRepository;
            _roundRepository = roundRepository;
            _teamRepository = teamRepository;
            _fixtureRepository = fixtureRepository;
        }

        public async Task<VoidResult> Handle(
            AddSeasonWithRoundsAndFixturesCommand command, CancellationToken cancellationToken
        ) {
            var season = command.Season;
            var leagueDto = season.League;

            var league = await _leagueRepository.FindById(leagueDto.Id);
            if (league == null) {
                league = new League(
                    id: leagueDto.Id,
                    name: leagueDto.Name,
                    type: leagueDto.Type,
                    isCup: leagueDto.IsCup,
                    logoUrl: leagueDto.LogoUrl
                );

                league.AddSeason(new Season(
                    id: season.Id,
                    name: season.Name
                ));

                _leagueRepository.Create(league);
            } else {
                if (!league.Seasons.Any(s => s.Id == season.Id)) {
                    // @@TODO: Check that EF Core detects this season addition.
                    // Do we need to track league for this or AsNoTracking is fine? Most likely have to track.
                    league.AddSeason(new Season(
                        id: season.Id,
                        name: season.Name
                    ));
                }
            }

            var rounds = await _roundRepository.FindById(season.Rounds.Select(r => r.Id));
            foreach (var seasonRound in season.Rounds) {
                var round = rounds.FirstOrDefault(r => r.Id == seasonRound.Id);
                if (round == null) {
                    _roundRepository.Create(new Round(
                        id: seasonRound.Id,
                        seasonId: season.Id,
                        name: seasonRound.Name,
                        startDate: seasonRound.StartDate != null ?
                            new DateTimeOffset(seasonRound.StartDate.Value).ToUnixTimeMilliseconds() :
                            null,
                        endDate: seasonRound.EndDate != null ?
                            new DateTimeOffset(seasonRound.EndDate.Value).ToUnixTimeMilliseconds() :
                            null,
                        isCurrent: seasonRound.Id == season.CurrentRoundId
                    ));
                } else {
                    round.ChangeStartDate(seasonRound.StartDate != null ?
                        new DateTimeOffset(seasonRound.StartDate.Value).ToUnixTimeMilliseconds() :
                        null
                    );
                    round.ChangeEndDate(seasonRound.EndDate != null ?
                        new DateTimeOffset(seasonRound.EndDate.Value).ToUnixTimeMilliseconds() :
                        null
                    );
                    round.SetCurrent(round.Id == season.CurrentRoundId);
                }
            }

            var fixtureTeams = season.Rounds
                .SelectMany(r => r.Fixtures.SelectMany(f => new[] { f.HomeTeam, f.GuestTeam }))
                .Distinct()
                .ToList();

            var teams = await _teamRepository.FindById(fixtureTeams.Select(t => t.Id));

            foreach (var fixtureTeam in fixtureTeams) {
                if (!teams.Any(t => t.Id == fixtureTeam.Id)) {
                    _teamRepository.Create(new Team(
                        id: fixtureTeam.Id,
                        name: fixtureTeam.Name,
                        countryId: fixtureTeam.CountryId,
                        logoUrl: fixtureTeam.LogoUrl
                    ));
                }
            }

            var fixtureDtos = season.Rounds.SelectMany(r => r.Fixtures).ToList();

            var fixtures = await _fixtureRepository.FindById(fixtureDtos.Select(f => f.Id));

            foreach (var fixtureDto in fixtureDtos) {
                var fixture = fixtures.FirstOrDefault(f => f.Id == fixtureDto.Id);
                if (fixture == null) {
                    _fixtureRepository.Create(new Fixture(
                        id: fixtureDto.Id,
                        seasonId: season.Id,
                        roundId: fixtureDto.RoundId,
                        startTime: new DateTimeOffset(fixtureDto.StartTime).ToUnixTimeMilliseconds(),
                        status: fixtureDto.Status,
                        homeTeamId: fixtureDto.HomeTeam.Id,
                        guestTeamId: fixtureDto.GuestTeam.Id,
                        gameTime: new GameTime(
                            minute: fixtureDto.GameTime.Minute,
                            extraTimeMinute: fixtureDto.GameTime.ExtraTimeMinute,
                            addedTimeMinute: fixtureDto.GameTime.AddedTimeMinute
                        ),
                        score: new Score(
                            localTeam: fixtureDto.Score.LocalTeam,
                            visitorTeam: fixtureDto.Score.VisitorTeam,
                            ht: fixtureDto.Score.HT,
                            ft: fixtureDto.Score.FT,
                            et: fixtureDto.Score.ET,
                            ps: fixtureDto.Score.PS
                        )
                    ));
                } else {
                    fixture.ChangeStartTime(new DateTimeOffset(fixtureDto.StartTime).ToUnixTimeMilliseconds());
                    fixture.ChangeStatus(fixtureDto.Status);
                    fixture.ChangeGameTime(new GameTime(
                        minute: fixtureDto.GameTime.Minute,
                        extraTimeMinute: fixtureDto.GameTime.ExtraTimeMinute,
                        addedTimeMinute: fixtureDto.GameTime.AddedTimeMinute
                    ));
                    fixture.ChangeScore(new Score(
                        localTeam: fixtureDto.Score.LocalTeam,
                        visitorTeam: fixtureDto.Score.VisitorTeam,
                        ht: fixtureDto.Score.HT,
                        ft: fixtureDto.Score.FT,
                        et: fixtureDto.Score.ET,
                        ps: fixtureDto.Score.PS
                    ));
                }
            }

            await _fixtureRepository.SaveChanges(cancellationToken);

            return VoidResult.Instance;
        }
    }
}
