using System.Text.Json.Serialization;

using MatchPredictions.Application.Common.Dto;
using MatchPredictions.Domain.Aggregates.Fixture;

namespace MatchPredictions.Application.Playtime.Commands.SubmitMatchPredictions {
    public class AlreadyStartedFixtureDto {
        public long Id { get; init; }
        public long StartTime { get; init; }
        public string Status { get; init; }
        public GameTimeDto GameTime { get; set; }
        [JsonIgnore]
        public GameTime GameTimeEntity { get; init; }
        public ScoreDto Score { get; set; }
        [JsonIgnore]
        public Score ScoreEntity { get; init; }
    }
}
