using System;
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

        public void SetVimeoProjectIdFor(long fixtureId, long teamId, string vimeoProjectId) {
            _ensureTransaction();

            _transaction.StringSetAsync(
                $"f:{fixtureId}.t:{teamId}.video-reactions-vimeo-project-id",
                vimeoProjectId
            );
        }

        public Task<bool> TryReserveSlotFor__immediate(long fixtureId, long teamId, long userId) {
            return _redis.GetDatabase().SetAddAsync(
                $"f:{fixtureId}.t:{teamId}.video-reaction-author-ids",
                userId
            );
        }

        public Task ReleaseSlotFor__immediate(long fixtureId, long teamId, long userId) {
            return _redis.GetDatabase().SetRemoveAsync(
                $"f:{fixtureId}.t:{teamId}.video-reaction-author-ids",
                userId
            );
        }

        public void Create(VideoReaction videoReaction) {
            _ensureTransaction();

            var fixtureIdentifier = $"f:{videoReaction.FixtureId}.t:{videoReaction.TeamId}";
            var authorIdentifier = $"vra:{videoReaction.AuthorId}";

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
                    new HashEntry($"{authorIdentifier}.{nameof(VideoReaction.Title)}", videoReaction.Title),
                    new HashEntry($"{authorIdentifier}.{nameof(VideoReaction.AuthorUsername)}", videoReaction.AuthorUsername),
                    new HashEntry($"{authorIdentifier}.{nameof(VideoReaction.VideoId)}", videoReaction.VideoId)
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

        public void DeleteAllFor(long fixtureId, long teamId) {
            _ensureTransaction();

            var keysToDelete = new RedisKey[] {
                $"f:{fixtureId}.t:{teamId}.video-reactions",
                $"f:{fixtureId}.t:{teamId}.video-reactions-vimeo-project-id",
                $"f:{fixtureId}.t:{teamId}.video-reaction-author-ids",
                $"f:{fixtureId}.t:{teamId}.video-reaction-author-ids.by-rating",
                $"f:{fixtureId}.t:{teamId}.video-reaction-author-ids.by-date"
            };

            //_transaction.KeyDeleteAsync(keysToDelete);

            foreach (var key in keysToDelete) {
                _transaction.KeyExpireAsync(key, TimeSpan.FromMinutes(10));
            }
        }
    }
}
