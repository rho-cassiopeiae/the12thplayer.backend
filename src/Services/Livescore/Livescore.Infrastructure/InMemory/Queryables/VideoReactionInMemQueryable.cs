using System.Threading.Tasks;

using StackExchange.Redis;

using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.InMemory.Queryables {
    public class VideoReactionInMemQueryable : IVideoReactionInMemQueryable {
        private readonly ConnectionMultiplexer _redis;

        public VideoReactionInMemQueryable(ConnectionMultiplexer redis) {
            _redis = redis;
        }

        public async Task<string> GetVimeoProjectIdFor(long fixtureId, long teamId) {
            var result = await _redis.GetDatabase().StringGetAsync(
                $"f:{fixtureId}.t:{teamId}.video-reactions-vimeo-project-id"
            );

            return result;
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
