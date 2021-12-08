﻿using System;
using System.Threading.Tasks;

using StackExchange.Redis;

using Livescore.Domain.Aggregates.VideoReaction;
using Livescore.Domain.Base;

namespace Livescore.Infrastructure.InMemory.Repositories {
    public class VideoReactionInMemRepository : IVideoReactionInMemRepository {
        private readonly ConnectionMultiplexer _redis;

        private IInMemUnitOfWork _unitOfWork;
        private ITransaction _transaction;

        public VideoReactionInMemRepository(ConnectionMultiplexer redis) {
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

        public void Create(VideoReaction videoReaction) {
            _ensureTransaction();

            var fixtureIdentifier = $"f:{videoReaction.FixtureId}.t:{videoReaction.TeamId}";
            var authorIdentifier = $"vra:{videoReaction.AuthorId}";

            _transaction.AddCondition(Condition.SortedSetNotContains(
                $"{fixtureIdentifier}.video-reaction-author-ids.by-rating", videoReaction.AuthorId
            ));

            _transaction.SortedSetAddAsync(
                $"{fixtureIdentifier}.video-reaction-author-ids.by-rating",
                videoReaction.AuthorId,
                videoReaction.Rating
            );

            _transaction.SortedSetAddAsync(
                $"{fixtureIdentifier}.video-reaction-author-ids.by-date",
                videoReaction.AuthorId,
                videoReaction.PostedAt
            );

            _transaction.HashSetAsync(
                $"{fixtureIdentifier}.video-reactions",
                new[] {
                    new HashEntry($"{authorIdentifier}.{nameof(videoReaction.Title)}", videoReaction.Title),
                    new HashEntry($"{authorIdentifier}.{nameof(videoReaction.AuthorUsername)}", videoReaction.AuthorUsername),
                    new HashEntry($"{authorIdentifier}.{nameof(videoReaction.VideoId)}", videoReaction.VideoId),
                    new HashEntry($"{authorIdentifier}.{nameof(videoReaction.ThumbnailUrl)}", videoReaction.ThumbnailUrl)
                }
            );
        }
        
        public void UpdateRatingFor(long fixtureId, long teamId, long authorId, int incrementRatingBy) {
            _ensureTransaction();

            _transaction.SortedSetIncrementAsync(
                $"f:{fixtureId}.t:{teamId}.video-reaction-author-ids.by-rating",
                authorId,
                incrementRatingBy
            );
        }
    }
}