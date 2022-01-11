using System;

namespace MessageBus.Contracts.Common.Dto {
    public class FixtureForMatchPredictionDto {
        public long Id { get; set; }
        public long RoundId { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }
        public GameTimeDto GameTime { get; set; }
        public ScoreDto Score { get; set; }
        public TeamDto HomeTeam { get; set; }
        public TeamDto GuestTeam { get; set; }
    }
}
