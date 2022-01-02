using System.Collections.Generic;

using Livescore.Application.Common.Dto;

namespace Livescore.Application.Livescore.Fixture.Common.Dto {
    // @@NOTE: Can't inherit from FixtureSummaryDto, because EF Core throws an exception
    // saying that it can't configure summary to be keyless when there is a key defined.
    public class FixtureFullDto {
        public long Id { get; set; }
        public bool HomeStatus { get; set; }
        public long StartTime { get; set; }
        public string Status { get; set; }
        public GameTimeDto GameTime { get; set; }
        public ScoreDto Score { get; set; }
        public string LeagueName { get; set; }
        public string LeagueLogoUrl { get; set; }
        public long OpponentTeamId { get; set; }
        public string OpponentTeamName { get; set; }
        public string OpponentTeamLogoUrl { get; set; }
        public string VenueName { get; set; }
        public string VenueImageUrl { get; set; }
        public string RefereeName { get; set; }
        public IEnumerable<TeamColorDto> Colors { get; set; }
        public IEnumerable<TeamLineupDto> Lineups { get; set; }
        public IEnumerable<TeamMatchEventsDto> Events { get; set; }
        public IEnumerable<TeamStatsDto> Stats { get; set; }
    }
}
