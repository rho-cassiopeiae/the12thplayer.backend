using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Worker.Application.Common.Interfaces;
using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection {
    public class FetchTeamFinishedFixturesJob : OneOffJob {
        private readonly IFootballDataProvider _footballDataProvider;
        private readonly ILivescoreSeeder _livescoreSeeder;

        public FetchTeamFinishedFixturesJob(
            ILogger<FetchTeamFinishedFixturesJob> logger,
            IFootballDataProvider footballDataProvider,
            ILivescoreSeeder livescoreSeeder
        ) : base(logger) {
            _footballDataProvider = footballDataProvider;
            _livescoreSeeder = livescoreSeeder;
        }

        protected override async Task<IDictionary<string, object>> _execute() {
            var teamId = long.Parse(_context.MergedJobDataMap.GetString("TeamId"));
            var startDate = _context.MergedJobDataMap.GetString("StartDate");
            string endDate;
            if (_context.MergedJobDataMap.TryGetValue("EndDate", out object value)) {
                endDate = (string) value;
            } else {
                endDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            }

            var addDummyPlayerRatings = false;
            if (_context.MergedJobDataMap.TryGetValue("AddDummyPlayerRatings", out value)) {
                addDummyPlayerRatings = bool.Parse((string) value);
            }

            var fixtures = (await _footballDataProvider.GetTeamFinishedFixtures(teamId, startDate, endDate))
                .ToList();

            var seasonIds = fixtures
                .Where(fixture => fixture.SeasonId != null)
                .Select(fixture => fixture.SeasonId.Value)
                .Distinct();

            var seasons = await _footballDataProvider.GetSeasons(seasonIds);

            var playerLineupEntries = fixtures
                .SelectMany(fixture => {
                    var lineup = fixture.Lineups.First(lineup => lineup.TeamId == teamId);
                    return (lineup.StartingXI ?? new List<TeamLineupDto.PlayerDto>())
                        .Concat(lineup.Subs ?? new List<TeamLineupDto.PlayerDto>());
                })
                .Reverse() // most recent first
                .ToList();

            var playerIds = playerLineupEntries
                .Select(p => p.Id)
                .Distinct();

            var players = (await _footballDataProvider.GetPlayers(playerIds)).ToList();
            
            foreach (var player in players) {
                var lastPlayerLineupEntry = playerLineupEntries.First(p => p.Id == player.Id);
                player.Number = lastPlayerLineupEntry.Number;
                player.LastLineupAt = lastPlayerLineupEntry.FixtureStartTime;
            }

            foreach (var playerLineupEntry in playerLineupEntries) {
                var player = players.First(p => p.Id == playerLineupEntry.Id);
                playerLineupEntry.FirstName = player.FirstName;
                playerLineupEntry.LastName = player.LastName;
                playerLineupEntry.DisplayName = player.DisplayName;
                playerLineupEntry.ImageUrl = player.ImageUrl;
            }

            var opponentPlayerLineupEntries = fixtures
                .SelectMany(fixture => {
                    var lineup = fixture.Lineups.First(lineup => lineup.TeamId == fixture.OpponentTeam.Id);
                    return (lineup.StartingXI ?? new List<TeamLineupDto.PlayerDto>())
                        .Concat(lineup.Subs ?? new List<TeamLineupDto.PlayerDto>());
                })
                .ToList();

            var opponentPlayerIds = opponentPlayerLineupEntries
                .Select(p => p.Id)
                .Distinct();

            var opponentPlayers = (await _footballDataProvider.GetPlayers(opponentPlayerIds)).ToList();

            foreach (var opponentPlayerLineupEntry in opponentPlayerLineupEntries) {
                var player = opponentPlayers.First(p => p.Id == opponentPlayerLineupEntry.Id);
                opponentPlayerLineupEntry.FirstName = player.FirstName;
                opponentPlayerLineupEntry.LastName = player.LastName;
                opponentPlayerLineupEntry.DisplayName = player.DisplayName;
                opponentPlayerLineupEntry.ImageUrl = player.ImageUrl;
            }

            IEnumerable<FixturePlayerRatingsDto> fixturePlayerRatings = null;
            if (addDummyPlayerRatings) {
                fixturePlayerRatings = _createDummyPlayerRatings(fixtures, teamId);
            }

            await _livescoreSeeder.AddTeamFinishedFixtures(teamId, fixtures, seasons, players, fixturePlayerRatings);

            return new Dictionary<string, object> {
                ["TeamId"] = teamId.ToString()
            };
        }

        IEnumerable<FixturePlayerRatingsDto> _createDummyPlayerRatings(List<FixtureDto> fixtures, long teamId) {
            return fixtures.Select(fixture => {
                var lineup = fixture.Lineups.First(lineup => lineup.TeamId == teamId);
                var playerRatings = (lineup.StartingXI?.ToList() ?? new List<TeamLineupDto.PlayerDto>())
                    .Select(player => new PlayerRatingDto {
                        ParticipantKey = $"p:{player.Id}",
                        Rating = player.Rating
                    })
                    .ToList();

                var events = fixture.Events.First(events => events.TeamId == teamId).Events;
                if (events != null) {
                    foreach (var @event in events.Where(e => e.Type == "substitution" && e.PlayerId != null)) {
                        var player = lineup.Subs?.FirstOrDefault(player => player.Id == @event.PlayerId.Value);
                        if (player != null) {
                            playerRatings.Add(new PlayerRatingDto {
                                ParticipantKey = $"s:{player.Id}",
                                Rating = player.Rating
                            });
                        }
                    }
                }

                var random = new Random();
                int baseTotalVoters = random.Next(51, 107);

                playerRatings.ForEach(pr => {
                    var rating = pr.Rating ?? 0.0f;
                    pr.TotalVoters = rating == 0.0f ? 0 : random.Next(baseTotalVoters - 7, baseTotalVoters + 7);
                    pr.TotalRating = (int) (rating * pr.TotalVoters);
                });

                if (lineup.Manager != null) {
                    int avgTotalRating = 0;
                    int avgTotalVoters = 0;
                    if (playerRatings.Count > 0) {
                        avgTotalRating = (int) playerRatings.Average(pr => pr.TotalRating);
                        avgTotalVoters = (int) playerRatings.Average(pr => pr.TotalVoters);
                    }

                    playerRatings.Add(new PlayerRatingDto {
                        ParticipantKey = $"m:{lineup.Manager.Id}",
                        TotalRating = avgTotalRating,
                        TotalVoters = avgTotalVoters
                    });
                }

                return new FixturePlayerRatingsDto {
                    FixtureId = fixture.Id,
                    TeamId = teamId,
                    PlayerRatings = playerRatings
                };
            });
        }
    }
}
