using MatchPredictions.Application.Common.Dto;

namespace MatchPredictions.Application.Playtime.Commands.SubmitMatchPredictions {
    public class AlreadyStartedFixtureDto {
        public long Id { get; init; }
        public long StartTime { get; init; }
        public string Status { get; init; }
        public GameTimeDto GameTime { get; init; }
        public ScoreDto Score { get; init; }
    }
}
