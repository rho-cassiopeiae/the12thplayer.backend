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
    }
}
