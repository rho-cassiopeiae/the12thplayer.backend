using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using StackExchange.Redis;

using Livescore.Domain.Aggregates.Discussion;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.InMemory.Queryables {
    public class DiscussionInMemQueryable : IDiscussionInMemQueryable {
        private readonly ConnectionMultiplexer _redis;

        private readonly int _getEntriesCount;

        public DiscussionInMemQueryable(
            IConfiguration configuration,
            ConnectionMultiplexer redis
        ) {
            _redis = redis;
            _getEntriesCount = configuration.GetValue<int>("Discussion:GetEntriesCount") + 1;
        }

        public async Task<bool> CheckActive(long fixtureId, long teamId, Guid discussionId) {
            var value = await _redis.GetDatabase().HashGetAsync(
                $"f:{fixtureId}.t:{teamId}.discussions",
                $"d:{discussionId}.{nameof(Discussion.Active)}"
            );

            return !value.IsNull && (int) value == 1;
        }

        public async Task<IEnumerable<Discussion>> GetAllFor(long fixtureId, long teamId) {
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

                    return e1.Name.CompareTo(e2.Name);
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

        public async Task<IEnumerable<DiscussionEntry>> GetEntriesFor(
            long fixtureId, long teamId, Guid discussionId, string startFromEntryId = null
        ) {
            var entries = await _redis.GetDatabase().StreamRangeAsync(
                $"f:{fixtureId}.t:{teamId}.d:{discussionId}",
                "-",
                startFromEntryId ?? "+",
                _getEntriesCount,
                Order.Descending
            );

            return entries.Select(e => new DiscussionEntry(
                id: e.Id,
                userId: (long) e.Values.First(nv => nv.Name == nameof(DiscussionEntry.UserId)).Value,
                username: e.Values.First(nv => nv.Name == nameof(DiscussionEntry.Username)).Value,
                body: e.Values.First(nv => nv.Name == nameof(DiscussionEntry.Body)).Value
            ));
        }
    }
}
