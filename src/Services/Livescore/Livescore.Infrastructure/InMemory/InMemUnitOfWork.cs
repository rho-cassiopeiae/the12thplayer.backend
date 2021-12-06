using System;
using System.Threading.Tasks;

using StackExchange.Redis;

using Livescore.Domain.Base;

namespace Livescore.Infrastructure.InMemory {
    public class InMemUnitOfWork : IInMemUnitOfWork {
        private readonly ConnectionMultiplexer _redis;

        private ITransaction _transaction;

        public ITransaction Transaction => _transaction;

        public InMemUnitOfWork(ConnectionMultiplexer redis) {
            _redis = redis;
        }

        public void Begin() {
            if (_transaction != null) {
                throw new InvalidOperationException(
                    "Cannot start a transaction while another is still in progress"
                );
            }

            _transaction = _redis.GetDatabase().CreateTransaction();
        }

        public async Task<bool> Commit() {
            if (_transaction == null) {
                throw new InvalidOperationException(
                    "There is no active transaction to commit"
                );
            }

            bool committed = await _transaction.ExecuteAsync();
            _transaction = null;

            return committed;
        }
    }
}
