using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        public async Task<IEnumerable<PlayerRating>> FindAllFor(long fixtureId, long teamId) {
            var entries = await _redis.GetDatabase().HashGetAllAsync(
                $"f:{fixtureId}.t:{teamId}.player-ratings"
            );

            Array.Sort(
                entries,
                (e1, e2) => {
                    // m:4891.total-rating
                    // m:4891.total-voters
                    // p:14611.total-rating
                    // p:14611.total-voters
                    // s:789.total-rating
                    // s:789.total-voters

                    var e1NameSplit = e1.Name.ToString().Split(':', '.');
                    var e2NameSplit = e2.Name.ToString().Split(':', '.');
                    var c = e1NameSplit[0].CompareTo(e2NameSplit[0]);
                    if (c != 0) {
                        return c;
                    }

                    c = long.Parse(e1NameSplit[1]).CompareTo(long.Parse(e2NameSplit[1]));
                    if (c != 0) {
                        return c;
                    }

                    return e1NameSplit[2].CompareTo(e2NameSplit[2]);
                }
            );

            var playerRatings = new List<PlayerRating>(entries.Length / 2);
            for (int i = 0; i < entries.Length; i += 2) {
                var entryTotalRating = entries[i];
                var entryTotalVoters = entries[i + 1];

                playerRatings.Add(new PlayerRating(
                    fixtureId: fixtureId,
                    teamId: teamId,
                    participantKey: entryTotalRating.Name.ToString().Split('.')[0],
                    totalRating: (int) entryTotalRating.Value,
                    totalVoters: (int) entryTotalVoters.Value
                ));
            }

            return playerRatings;
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
                    (int) newRating - (int) oldRating.Value
                );
            } else {
                _transaction.HashIncrementAsync(
                    $"{fixtureIdentifier}.player-ratings",
                    $"{participantKey}.total-rating",
                    (int) newRating
                );
                _transaction.HashIncrementAsync(
                    $"{fixtureIdentifier}.player-ratings",
                    $"{participantKey}.total-voters",
                    1
                );
            }
        }

        public void DeleteAllFor(long fixtureId, long teamId) {
            _ensureTransaction();

            //_transaction.KeyDeleteAsync($"f:{fixtureId}.t:{teamId}.player-ratings");
            _transaction.KeyExpireAsync($"f:{fixtureId}.t:{teamId}.player-ratings", TimeSpan.FromMinutes(10));
        }
    }
}
