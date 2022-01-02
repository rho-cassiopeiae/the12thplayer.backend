using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using StackExchange.Redis;

using Livescore.Domain.Aggregates.UserVote;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.InMemory.Queryables {
    public class UserVoteInMemQueryable : IUserVoteInMemQueryable {
        private readonly ConnectionMultiplexer _redis;

        public UserVoteInMemQueryable(ConnectionMultiplexer redis) {
            _redis = redis;
        }

        public async Task<UserVote> GetPlayerRatingsFor(
            long userId, long fixtureId, long teamId,
            List<string> fixtureParticipantKeys
        ) {
            var result = await _redis.GetDatabase().ScriptEvaluateAsync(
                @"
                    local field = ARGV[1]
                    local res = {}
                    for i, key in ipairs(KEYS) do
                        res[#res+1] = redis.call('HGET', key, field)
                    end

                    return res
                ",
                keys: fixtureParticipantKeys
                    .Select(participantKey => (RedisKey) $"f:{fixtureId}.t:{teamId}.{participantKey}.user-votes")
                    .ToArray(),
                values: new RedisValue[] { $"u:{userId}" }
            );

            UserVote userVote = null;
            
            var values = (RedisValue[]) result;
            for (int i = 0; i < fixtureParticipantKeys.Count; ++i) {
                var participantKey = fixtureParticipantKeys[i];
                var value = values[i];
                if (!value.IsNull) {
                    userVote ??= new UserVote(
                        userId: userId,
                        fixtureId: fixtureId,
                        teamId: teamId
                    );
                    userVote.AddPlayerRating(participantKey, (float) value);
                }
            }

            return userVote;
        }

        public async Task<UserVote> GetVideoReactionVotesFor(
            long userId, long fixtureId, long teamId,
            List<long> authorIds
        ) {
            var result = await _redis.GetDatabase().ScriptEvaluateAsync(
                @"
                    local field = ARGV[1]
                    local res = {}
                    for i, key in ipairs(KEYS) do
                        res[#res+1] = redis.call('HGET', key, field)
                    end

                    return res
                ",
                keys: authorIds
                    .Select(authorId => (RedisKey) $"f:{fixtureId}.t:{teamId}.vra:{authorId}.user-votes")
                    .ToArray(),
                values: new RedisValue[] { $"u:{userId}" }
            );

            UserVote userVote = null;

            var values = (RedisValue[]) result;
            for (int i = 0; i < authorIds.Count; ++i) {
                var authorId = authorIds[i];
                var value = values[i];
                if (!value.IsNull) {
                    userVote ??= new UserVote(
                        userId: userId,
                        fixtureId: fixtureId,
                        teamId: teamId
                    );
                    userVote.AddVideoReactionVote(authorId, (short) value);
                }
            }

            return userVote;
        }
    }
}
