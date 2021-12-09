using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            var values = await _redis.GetDatabase().HashGetAsync(
                $"f:{fixtureId}.t:{teamId}.player-ratings",
                new RedisValue[] {
                    $"{participantKey}.total-rating",
                    $"{participantKey}.total-voters"
                }
            );

            return new PlayerRating(
                fixtureId: fixtureId,
                teamId: teamId,
                participantKey: participantKey,
                totalRating: (int) values[0],
                totalVoters: (int) values[1]
            );
        }

        public async Task<IEnumerable<PlayerRating>> GetAllFor(long fixtureId, long teamId) {
            var entries = await _redis.GetDatabase().HashGetAllAsync(
                $"f:{fixtureId}.t:{teamId}.player-ratings"
            );

            Array.Sort(
                entries,
                (e1, e2) => {
                    // m:4891.total-rating
                    // m:4891.total-voters
                    // p:14611.total-rating
                    // p:14611.total-voters
                    // s:789.total-rating
                    // s:789.total-voters

                    var e1NameSplit = e1.Name.ToString().Split(':', '.');
                    var e2NameSplit = e2.Name.ToString().Split(':', '.');
                    var c = e1NameSplit[0].CompareTo(e2NameSplit[0]);
                    if (c != 0) {
                        return c;
                    }

                    c = long.Parse(e1NameSplit[1]).CompareTo(long.Parse(e2NameSplit[1]));
                    if (c != 0) {
                        return c;
                    }

                    return e1NameSplit[2].CompareTo(e2NameSplit[2]);
                }
            );

            var playerRatings = new List<PlayerRating>(entries.Length / 2);
            for (int i = 0; i < entries.Length; i += 2) {
                var entryTotalRating = entries[i];
                var entryTotalVoters = entries[i + 1];

                playerRatings.Add(new PlayerRating(
                    fixtureId: fixtureId,
                    teamId: teamId,
                    participantKey: entryTotalRating.Name.ToString().Split('.')[0],
                    totalRating: (int) entryTotalRating.Value,
                    totalVoters: (int) entryTotalVoters.Value
                ));
            }

            return playerRatings;
        }
    }
}
