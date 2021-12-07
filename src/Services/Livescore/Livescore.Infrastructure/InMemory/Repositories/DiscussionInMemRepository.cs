using System;
using System.Threading.Tasks;

using StackExchange.Redis;

using Livescore.Domain.Aggregates.Discussion;
using Livescore.Domain.Base;

namespace Livescore.Infrastructure.InMemory.Repositories {
    public class DiscussionInMemRepository : IDiscussionInMemRepository {
        private readonly ConnectionMultiplexer _redis;

        private IInMemUnitOfWork _unitOfWork;
        private ITransaction _transaction;

        public DiscussionInMemRepository(ConnectionMultiplexer redis) {
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
        
        public void Create(Discussion discussion) {
            _ensureTransaction();

            var fixtureIdentifier = $"f:{discussion.FixtureId}.t:{discussion.TeamId}";

            _transaction.HashSetAsync(
                $"{fixtureIdentifier}.discussions",
                new[] {
                    new HashEntry($"d:{discussion.Id}.{nameof(discussion.Name)}", discussion.Name),
                    new HashEntry($"d:{discussion.Id}.{nameof(discussion.Active)}", discussion.Active ? 1 : 0)
                }
            );

            foreach (var entry in discussion.Entries) {
                _transaction.StreamAddAsync(
                    $"{fixtureIdentifier}.d:{discussion.Id}",
                    new[] {
                        new NameValueEntry(nameof(entry.Username), entry.Username),
                        new NameValueEntry(nameof(entry.Body), entry.Body)
                    },
                    messageId: entry.Id
                );
            }

            _transaction.StreamAddAsync(
                "discussions",
                new[] {
                    new NameValueEntry("identifier", $"{fixtureIdentifier}.d:{discussion.Id}"),
                    new NameValueEntry("command", "sub")
                }
            );
        }
    }
}
