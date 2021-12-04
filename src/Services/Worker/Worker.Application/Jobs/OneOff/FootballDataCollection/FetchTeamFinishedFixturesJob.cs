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

            var fixtures = (await _footballDataProvider.GetTeamFinishedFixtures(
                teamId, startDate, endDate
            )).ToList();

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
                .Reverse()
                .ToList();

            var playerIds = playerLineupEntries
                .Select(p => p.Id)
                .Distinct();

            var players = (await _footballDataProvider.GetPlayers(playerIds)).ToList();
            
            foreach (var player in players) {
                var lastPlayerLineupEntry = playerLineupEntries.First(p => p.Id == player.Id);
                player.Number = lastPlayerLineupEntry.Number;
                player.LastLineupAt = lastPlayerLineupEntry.FixtureStartTime.Value;
            }

            foreach (var playerLineupEntry in playerLineupEntries) {
                var player = players.First(p => p.Id == playerLineupEntry.Id);
                playerLineupEntry.FirstName = player.FirstName;
                playerLineupEntry.LastName = player.LastName;
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
                opponentPlayerLineupEntry.ImageUrl = player.ImageUrl;
            }

            await _livescoreSeeder.AddTeamFinishedFixtures(teamId, fixtures, seasons, players);

            return new Dictionary<string, object> {
                ["TeamId"] = teamId.ToString()
            };
        }
    }
}
