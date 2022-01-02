﻿using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using StackExchange.Redis;

using Livescore.Application.Common.Interfaces;
using Livescore.Domain.Aggregates.VideoReaction;
using Livescore.Application.Livescore.VideoReaction.Queries.GetVideoReactionsForFixture;

namespace Livescore.Infrastructure.InMemory.Queryables {
    public class VideoReactionInMemQueryable : IVideoReactionInMemQueryable {
        private readonly ConnectionMultiplexer _redis;

        private readonly int _getCount;

        public VideoReactionInMemQueryable(
            IConfiguration configuration,
            ConnectionMultiplexer redis
        ) {
            _redis = redis;
            _getCount = configuration.GetValue<int>("VideoReaction:GetCount");
        }

        public async Task<string> GetVimeoProjectIdFor(long fixtureId, long teamId) {
            var result = await _redis.GetDatabase().StringGetAsync(
                $"f:{fixtureId}.t:{teamId}.video-reactions-vimeo-project-id"
            );

            return result;
        }

        public async Task<(IEnumerable<VideoReaction> VideoReactions, int TotalPages)> GetFilteredAndPaginatedFor(
            long fixtureId, long teamId, VideoReactionFilter filter, int page
        ) {
            var fixtureIdentifier = $"f:{fixtureId}.t:{teamId}";
            var db = _redis.GetDatabase();

            var videoReactions = new List<VideoReaction>();
            if (filter == VideoReactionFilter.Top) {
                // @@NOTE: No error if out of range. Both are inclusive.
                var entries = await db.SortedSetRangeByRankWithScoresAsync(
                    $"{fixtureIdentifier}.video-reaction-author-ids.by-rating",
                    (page - 1) * _getCount,
                    page * _getCount - 1,
                    Order.Descending
                );
                foreach (var entry in entries) {
                    videoReactions.Add(
                        new VideoReaction(
                            fixtureId: fixtureId,
                            teamId: teamId,
                            authorId: (long) entry.Element,
                            rating: (int) entry.Score
                        )
                    );
                }
            } else { // newest first
                var authorIds = await db.SortedSetRangeByRankAsync(
                    $"{fixtureIdentifier}.video-reaction-author-ids.by-date",
                    (page - 1) * _getCount,
                    page * _getCount - 1,
                    Order.Descending
                );

                if (authorIds.Length > 0) {
                    var args = authorIds.Select(id => (object) (long) id).ToList();
                    args.Insert(0, $"{fixtureIdentifier}.video-reaction-author-ids.by-rating");
                    var result = await db.ExecuteAsync("ZMSCORE", args);
                    var ratings = (RedisValue[]) result;

                    for (int i = 0; i < authorIds.Length; ++i) {
                        videoReactions.Add(
                            new VideoReaction(
                                fixtureId: fixtureId,
                                teamId: teamId,
                                authorId: (long) authorIds[i],
                                rating: (int) ratings[i]
                            )
                        );
                    }
                }
            }

            if (videoReactions.Count > 0) {
                var fields = new RedisValue[videoReactions.Count * 3];
                for (int i = 0, j = 0; i < videoReactions.Count; ++i, j += 3) {
                    var authorId = videoReactions[i].AuthorId;
                    fields[j] =     $"vra:{authorId}.{nameof(VideoReaction.Title)}";
                    fields[j + 1] = $"vra:{authorId}.{nameof(VideoReaction.AuthorUsername)}";
                    fields[j + 2] = $"vra:{authorId}.{nameof(VideoReaction.VideoId)}";
                }

                var values = await db.HashGetAsync($"{fixtureIdentifier}.video-reactions", fields);
                for (int i = 0, j = 0; i < videoReactions.Count; ++i, j += 3) {
                    var videoReaction = videoReactions[i];
                    videoReaction.SetTitle(values[j]);
                    videoReaction.SetAuthorUsername(values[j + 1]);
                    videoReaction.SetVideoId(values[j + 2]);
                }
            }

            var totalVideoReactionCount = (int) await db.SortedSetLengthAsync(
                $"{fixtureIdentifier}.video-reaction-author-ids.by-rating"
            );
            var totalPages = totalVideoReactionCount / _getCount;
            if (totalPages == 0 || totalVideoReactionCount % _getCount > 0) {
                ++totalPages;
            }

            return (videoReactions, totalPages);
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
