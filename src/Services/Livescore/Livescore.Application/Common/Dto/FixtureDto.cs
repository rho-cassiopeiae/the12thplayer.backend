using System;
using System.Collections.Generic;

namespace Livescore.Application.Common.Dto {
    public class FixtureDto {
        public long Id { get; set; }
        public long? SeasonId { get; set; }
        public bool HomeStatus { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }
        public GameTimeDto GameTime { get; set; }
        public ScoreDto Score { get; set; }
        public string RefereeName { get; set; }
        public IEnumerable<TeamColorDto> Colors { get; set; }
        public IEnumerable<TeamLineupDto> Lineups { get; set; }
        public IEnumerable<TeamMatchEventsDto> Events { get; set; }
        public IEnumerable<TeamStatsDto> Stats { get; set; }
        public TeamDto OpponentTeam { get; set; }
        public VenueDto Venue { get; set; }
    }
}
