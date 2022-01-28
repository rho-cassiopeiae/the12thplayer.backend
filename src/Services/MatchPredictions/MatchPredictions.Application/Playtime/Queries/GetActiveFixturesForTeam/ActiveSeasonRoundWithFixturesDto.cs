using System.Collections.Generic;

namespace MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam {
    public class ActiveSeasonRoundWithFixturesDto {
        public long SeasonId { get; set; }
        public string LeagueName { get; set; }
        public string LeagueLogoUrl { get; set; }
        public long RoundId { get; set; }
        public string RoundName { get; set; }
        public IEnumerable<FixtureDto> Fixtures { get; set; }
    }
}
