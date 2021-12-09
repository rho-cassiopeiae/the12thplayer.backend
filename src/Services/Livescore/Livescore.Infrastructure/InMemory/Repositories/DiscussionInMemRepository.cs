using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        
        public async Task<IEnumerable<Discussion>> FindAllFor(long fixtureId, long teamId) {
            var entries = await _redis.GetDatabase().HashGetAllAsync(
                $"f:{fixtureId}.t:{teamId}.discussions"
            );

            Array.Sort(
                entries,
                (e1, e2) => {
                    // d:D6ADF015-A0EA-4F1A-8D1B-375E67DA0A58.Active
                    // d:D6ADF015-A0EA-4F1A-8D1B-375E67DA0A58.Name
                    // d:E9FA698A-7060-4AF1-A57F-D2657B45C78B.Active
                    // d:E9FA698A-7060-4AF1-A57F-D2657B45C78B.Name

                    var e1NameSplit = e1.Name.ToString().Split('.');
                    var e2NameSplit = e2.Name.ToString().Split('.');
                    var c = e1NameSplit[0].CompareTo(e2NameSplit[0]);
                    if (c != 0) {
                        return c;
                    }

                    return e1NameSplit[1].CompareTo(e2NameSplit[1]);
                }
            );

            var discussions = new List<Discussion>(entries.Length / 2);
            for (int i = 0; i < entries.Length; i += 2) {
                var entryActive = entries[i];
                var entryName = entries[i + 1];

                discussions.Add(new Discussion(
                    fixtureId: fixtureId,
                    teamId: teamId,
                    id: Guid.Parse(entryActive.Name.ToString().Split(':', '.')[1]),
                    name: entryName.Value,
                    active: entryActive.Value == 1
                ));
            }

            return discussions;
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

        public void Delete(long fixtureId, long teamId, List<Guid> discussionIds) {
            _ensureTransaction();

            var fixtureIdentifier = $"f:{fixtureId}.t:{teamId}";

            var keysToDelete = discussionIds
                .Select(discussionId => (RedisKey) $"{fixtureIdentifier}.d:{discussionId}")
                .ToList();

            keysToDelete.Add($"{fixtureIdentifier}.discussions");

            _transaction.KeyDeleteAsync(keysToDelete.ToArray());

            _transaction.StreamAddAsync(
                "discussions",
                new[] {
                    new NameValueEntry("identifier", fixtureIdentifier),
                    new NameValueEntry("command", "unsub")
                }
            );
        }
    }
}
