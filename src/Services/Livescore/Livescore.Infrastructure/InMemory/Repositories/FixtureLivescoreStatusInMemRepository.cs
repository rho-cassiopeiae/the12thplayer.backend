using System;
using System.Threading.Tasks;

using StackExchange.Redis;

using Livescore.Domain.Base;
using Livescore.Domain.Aggregates.FixtureLivescoreStatus;

namespace Livescore.Infrastructure.InMemory.Repositories {
    public class FixtureLivescoreStatusInMemRepository : IFixtureLivescoreStatusInMemRepository {
        private readonly ConnectionMultiplexer _redis;

        private IInMemUnitOfWork _unitOfWork;
        private ITransaction _transaction;

        public FixtureLivescoreStatusInMemRepository(ConnectionMultiplexer redis) {
            _redis = redis;
        }

        public void EnlistAsPartOf(IInMemUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
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

            return false;
        }

        private void _ensureTransaction() {
            if (_transaction == null) {
                _transaction = _unitOfWork?.Transaction ?? _redis.GetDatabase().CreateTransaction();
            }
        }

        public void SetOrUpdate(FixtureLivescoreStatus fixtureStatus) {
            _ensureTransaction();

            var fixtureIdentifier = $"fixture:{fixtureStatus.FixtureId}.team:{fixtureStatus.TeamId}";

            if (fixtureStatus.Active != null) {
                if (fixtureStatus.Active.Value) {
                    _transaction.SetAddAsync("fixtures.active", fixtureIdentifier);
                } else {
                    _transaction.SetRemoveAsync("fixtures.active", fixtureIdentifier);
                }
            }

            if (fixtureStatus.Ongoing != null) {
                if (fixtureStatus.Ongoing.Value) {
                    _transaction.SetAddAsync("fixtures.ongoing", fixtureIdentifier);
                } else {
                    _transaction.SetRemoveAsync("fixtures.ongoing", fixtureIdentifier);
                }
            }
        }
    }
}
