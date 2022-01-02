using Livescore.Application.Common.Dto;

namespace Livescore.Application.Livescore.Fixture.Common.Dto {
    public class FixtureSummaryDto {
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
    }
}
