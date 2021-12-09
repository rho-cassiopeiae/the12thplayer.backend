using System.Threading.Tasks;

using StackExchange.Redis;

using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.InMemory.Queryables {
    public class FixtureLivescoreStatusInMemQueryable : IFixtureLivescoreStatusInMemQueryable {
        private readonly ConnectionMultiplexer _redis;

        public FixtureLivescoreStatusInMemQueryable(ConnectionMultiplexer redis) {
            _redis = redis;
        }

        public Task<bool> CheckActive(long fixtureId, long teamId) {
            return _redis.GetDatabase().SetContainsAsync("fixtures.active", $"f:{fixtureId}.t:{teamId}");
        }
    }
}
