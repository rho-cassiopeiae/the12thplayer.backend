using System.Text.Json.Serialization;

using MatchPredictions.Application.Common.Dto;

namespace MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam {
    public class FixtureDto {
        public long Id { get; set; }
        [JsonIgnore]
        public long SeasonId { get; set; }
        [JsonIgnore]
        public long RoundId { get; set; }
        public long StartTime { get; set; }
        public string Status { get; set; }
        public GameTimeDto GameTime { get; set; }
        public ScoreDto Score { get; set; }
        public string HomeTeamName { get; set; }
        public string HomeTeamLogoUrl { get; set; }
        public string GuestTeamName { get; set; }
        public string GuestTeamLogoUrl { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public short? PredictedHomeTeamScore { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public short? PredictedGuestTeamScore { get; set; }
    }
}
