using System.Linq;
using System.Threading.Tasks;

using StackExchange.Redis;

using Livescore.Domain.Aggregates.PlayerRating;

namespace Livescore.Infrastructure.InMemory.Repositories {
    public class PlayerRatingInMemRepository : IPlayerRatingInMemRepository {
        private readonly ConnectionMultiplexer _redis;

        private ITransaction _transaction;

        public PlayerRatingInMemRepository(ConnectionMultiplexer redis) {
            _redis = redis;
        }

        public async ValueTask<bool> SaveChanges() {
            if (_transaction != null) {
                return await _transaction.ExecuteAsync();
            }

            return false;
        }

        private void _ensureTransaction() {
            if (_transaction == null) {
                _transaction = _redis.GetDatabase().CreateTransaction();
            }
        }

        public void CreateIfNotExists(PlayerRating playerRating) {
            _ensureTransaction();

            var fixtureIdentifier = $"fixture:{playerRating.FixtureId}.team:{playerRating.TeamId}";

            _transaction.HashSetAsync(
                $"{fixtureIdentifier}.player-ratings",
                $"{playerRating.ParticipantKey}.total-rating",
                playerRating.TotalRating,
                When.NotExists
            );
            _transaction.HashSetAsync(
                $"{fixtureIdentifier}.player-ratings",
                $"{playerRating.ParticipantKey}.total-voters",
                playerRating.TotalVoters,
                When.NotExists
            );
        }

        public async Task<PlayerRating> FindUserVoteForPlayer(
            long fixtureId, long teamId, long userId, string participantKey
        ) {
            var db = _redis.GetDatabase();

            var value = await db.HashGetAsync(
                $"fixture:{fixtureId}.team:{teamId}.user:{userId}.player-ratings", participantKey
            );

            var playerRating = new PlayerRating(
                fixtureId: fixtureId,
                teamId: teamId,
                participantKey: participantKey
            );

            playerRating.AddUserVote(new UserVote(
                userId: userId,
                currentRating: (int?) value
            ));

            return playerRating;
        }

        public void Update(PlayerRating playerRating) {
            _ensureTransaction();

            var fixtureIdentifier = $"fixture:{playerRating.FixtureId}.team:{playerRating.TeamId}";
            var userVote = playerRating.UserVotes.Single();

            if (userVote.CurrentRating != null) {
                _transaction.AddCondition(Condition.HashEqual(
                    $"{fixtureIdentifier}.user:{userVote.UserId}.player-ratings",
                    playerRating.ParticipantKey,
                    userVote.CurrentRating.Value
                ));

                _transaction.HashIncrementAsync(
                    $"{fixtureIdentifier}.player-ratings",
                    $"{playerRating.ParticipantKey}.total-rating",
                    userVote.NewRating - userVote.CurrentRating.Value
                );
            } else {
                _transaction.AddCondition(Condition.HashNotExists(
                    $"{fixtureIdentifier}.user:{userVote.UserId}.player-ratings",
                    playerRating.ParticipantKey
                ));

                _transaction.HashIncrementAsync(
                    $"{fixtureIdentifier}.player-ratings",
                    $"{playerRating.ParticipantKey}.total-rating",
                    userVote.NewRating
                );
                _transaction.HashIncrementAsync(
                    $"{fixtureIdentifier}.player-ratings",
                    $"{playerRating.ParticipantKey}.total-voters",
                    1
                );
            }

            _transaction.HashSetAsync(
                $"{fixtureIdentifier}.user:{userVote.UserId}.player-ratings",
                playerRating.ParticipantKey,
                userVote.NewRating
            );
        }
    }
}
