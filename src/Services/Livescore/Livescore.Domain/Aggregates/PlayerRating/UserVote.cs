using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.PlayerRating {
    public class UserVote : Entity {
        public long UserId { get; private set; }
        public int? CurrentRating { get; private set; }
        public int NewRating { get; private set; }

        public UserVote(long userId, int? currentRating, int newRating) {
            UserId = userId;
            CurrentRating = currentRating;
            NewRating = newRating;
        }

        public UserVote(long userId, int? currentRating) {
            UserId = userId;
            CurrentRating = currentRating;
        }
    }
}
