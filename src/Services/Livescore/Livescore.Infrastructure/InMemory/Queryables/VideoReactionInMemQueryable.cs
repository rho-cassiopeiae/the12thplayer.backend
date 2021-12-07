using System.Threading.Tasks;

using StackExchange.Redis;

using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.InMemory.Queryables {
    public class VideoReactionInMemQueryable : IVideoReactionInMemQueryable {
        private readonly ConnectionMultiplexer _redis;

        public VideoReactionInMemQueryable(ConnectionMultiplexer redis) {
            _redis = redis;
        }

        public async Task<int> GetRatingFor(long fixtureId, long teamId, long authorId) {
            return (int) (
                await _redis.GetDatabase().SortedSetScoreAsync(
                    $"f:{fixtureId}.t:{teamId}.video-reaction-author-ids.by-rating",
                    authorId
                )
            ).Value;
        }
    }
}
