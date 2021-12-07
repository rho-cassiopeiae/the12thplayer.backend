using System;
using System.Threading.Tasks;

using StackExchange.Redis;

using Livescore.Domain.Aggregates.PlayerRating;
using Livescore.Domain.Base;

namespace Livescore.Infrastructure.InMemory.Repositories {
    public class PlayerRatingInMemRepository : IPlayerRatingInMemRepository {
        private readonly ConnectionMultiplexer _redis;

        private IInMemUnitOfWork _unitOfWork;
        private ITransaction _transaction;

        public PlayerRatingInMemRepository(ConnectionMultiplexer redis) {
            _redis = redis;
        }

        public void EnlistAsPartOf(IInMemUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _transaction = unitOfWork.Transaction;
        }

        public async ValueTask<bool> SaveChanges() {
            if (_unitOfWork != null) {
                throw new InvalidOperationException(
                    $"This repository is enlisted as part of a unit of work, so its '{nameof(SaveChanges)}' method must not be invoked. " +
                    $"Instead, call '{nameof(IInMemUnitOfWork)}.{nameof(IInMemUnitOfWork.Commit)}'"
                );
            }

            if (_transaction != null) {
                return await _transaction.ExecuteAsync();
            }

            return true;
        }

        private void _ensureTransaction() {
            if (_transaction == null) {
                _transaction = _redis.GetDatabase().CreateTransaction();
            }
        }

        public void CreateIfNotExists(PlayerRating playerRating) {
            _ensureTransaction();

            var fixtureIdentifier = $"f:{playerRating.FixtureId}.t:{playerRating.TeamId}";

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

        public void UpdateOneFor(
            long fixtureId, long teamId, string participantKey,
            float? oldRating, float newRating
        ) {
            _ensureTransaction();

            var fixtureIdentifier = $"f:{fixtureId}.t:{teamId}";

            if (oldRating != null) {
                _transaction.HashIncrementAsync(
                    $"{fixtureIdentifier}.player-ratings",
                    $"{participantKey}.total-rating",
                    newRating - oldRating.Value
                );
            } else {
                _transaction.HashIncrementAsync(
                    $"{fixtureIdentifier}.player-ratings",
                    $"{participantKey}.total-rating",
                    newRating
                );
                _transaction.HashIncrementAsync(
                    $"{fixtureIdentifier}.player-ratings",
                    $"{participantKey}.total-voters",
                    1
                );
            }
        }
    }
}
