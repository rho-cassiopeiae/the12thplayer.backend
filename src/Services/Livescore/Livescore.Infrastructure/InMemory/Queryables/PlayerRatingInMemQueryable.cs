using System.Threading.Tasks;

using StackExchange.Redis;

using Livescore.Domain.Aggregates.PlayerRating;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.InMemory.Queryables {
    public class PlayerRatingInMemQueryable : IPlayerRatingInMemQueryable {
        private readonly ConnectionMultiplexer _redis;

        public PlayerRatingInMemQueryable(ConnectionMultiplexer redis) {
            _redis = redis;
        }

        public async Task<PlayerRating> GetRatingFor(long fixtureId, long teamId, string participantKey) {
            var db = _redis.GetDatabase();

            var values = await db.HashGetAsync($"fixture:{fixtureId}.team:{teamId}.player-ratings", new RedisValue[] {
                $"{participantKey}.total-rating",
                $"{participantKey}.total-voters"
            });

            return new PlayerRating(
                fixtureId: fixtureId,
                teamId: teamId,
                participantKey: participantKey,
                totalRating: (int) values[0],
                totalVoters: (int) values[1]
            );
        }
    }
}
