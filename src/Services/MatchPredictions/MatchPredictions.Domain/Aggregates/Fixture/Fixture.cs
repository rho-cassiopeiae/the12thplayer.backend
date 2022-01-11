using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.Fixture {
    public class Fixture : Entity, IAggregateRoot {
        public long Id { get; private set; }
        public long SeasonId { get; private set; }
        public long RoundId { get; private set; }
        public long StartTime { get; private set; }
        public string Status { get; private set; }
        public long HomeTeamId { get; private set; }
        public long GuestTeamId { get; private set; }
        public GameTime GameTime { get; private set; }
        public Score Score { get; private set; }

        public Fixture(
            long id, long seasonId, long roundId, long startTime, string status,
            long homeTeamId, long guestTeamId, GameTime gameTime, Score score
        ) {
            Id = id;
            SeasonId = seasonId;
            RoundId = roundId;
            StartTime = startTime;
            Status = status;
            HomeTeamId = homeTeamId;
            GuestTeamId = guestTeamId;
            GameTime = gameTime;
            Score = score;
        }

        public void ChangeStartTime(long startTime) {
            StartTime = startTime;
        }

        public void ChangeStatus(string status) {
            Status = status;
        }

        public void ChangeGameTime(GameTime gameTime) {
            GameTime = gameTime;
        }

        public void ChangeScore(Score score) {
            Score = score;
        }
    }
}
