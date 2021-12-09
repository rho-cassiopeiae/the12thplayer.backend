using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using StackExchange.Redis;

using Livescore.Domain.Aggregates.UserVote;
using Livescore.Domain.Base;

namespace Livescore.Infrastructure.InMemory.Repositories {
    public class UserVoteInMemRepository : IUserVoteInMemRepository {
        private readonly ConnectionMultiplexer _redis;

        private IInMemUnitOfWork _unitOfWork;
        private ITransaction _transaction;

        public UserVoteInMemRepository(ConnectionMultiplexer redis) {
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

        public async Task<UserVote> FindOneForFixtureParticipant(
            long userId, long fixtureId, long teamId, string participantKey
        ) {
            var value = await _redis.GetDatabase().HashGetAsync(
                $"f:{fixtureId}.t:{teamId}.{participantKey}.user-votes", $"u:{userId}"
            );

            var userVote = new UserVote(
                userId: userId,
                fixtureId: fixtureId,
                teamId: teamId
            );

            userVote.AddPlayerRating(participantKey, (float?) value);

            return userVote;
        }

        public async Task<IEnumerable<UserVote>> FindAllFor(
            long fixtureId, long teamId,
            List<string> fixtureParticipantKeys
        ) {
            var fixtureIdentifier = $"f:{fixtureId}.t:{teamId}";

            var db = _redis.GetDatabase();

            var tasks = fixtureParticipantKeys.Select(
                participantKey => db.HashGetAllAsync($"{fixtureIdentifier}.{participantKey}.user-votes")
            );

            HashEntry[][] results = await Task.WhenAll(tasks);

            var userVotes = new List<UserVote>();

            for (int i = 0; i < fixtureParticipantKeys.Count; ++i) {
                var participantKey = fixtureParticipantKeys[i];
                var entries = results[i];
                foreach (var entry in entries) {
                    var userId = long.Parse(entry.Name.ToString().Split(':')[1]);
                    var userVote = userVotes.FirstOrDefault(uv => uv.UserId == userId);
                    if (userVote == null) {
                        userVote = new UserVote(
                            userId: userId,
                            fixtureId: fixtureId,
                            teamId: teamId
                        );
                        userVotes.Add(userVote);
                    }
                    userVote.AddPlayerRating(participantKey, (float?) entry.Value);
                }
            }

            return userVotes;
        }

        public async Task<UserVote> FindOneForVideoReaction(
            long userId, long fixtureId, long teamId, long authorId
        ) {
            var value = await _redis.GetDatabase().HashGetAsync(
                $"f:{fixtureId}.t:{teamId}.vra:{authorId}.user-votes", $"u:{userId}"
            );

            var userVote = new UserVote(
                userId: userId,
                fixtureId: fixtureId,
                teamId: teamId
            );

            userVote.AddVideoReactionVote(authorId, (short?) value);

            return userVote;
        }

        public void UpdateOneForFixtureParticipant(UserVote userVote, float? oldRating) {
            _ensureTransaction();

            var participantKeyToRating = userVote.FixtureParticipantKeyToRating.Single();
            var hashKey = $"f:{userVote.FixtureId}.t:{userVote.TeamId}.{participantKeyToRating.Key}.user-votes";
            var hashField = $"u:{userVote.UserId}";

            if (oldRating != null) {
                _transaction.AddCondition(Condition.HashEqual(hashKey, hashField, oldRating.Value));
            } else {
                _transaction.AddCondition(Condition.HashNotExists(hashKey, hashField));
            }

            _transaction.HashSetAsync(hashKey, hashField, participantKeyToRating.Value.Value);
        }

        public void UpdateOneForVideoReaction(UserVote userVote, short? oldVote) {
            _ensureTransaction();

            var authorIdToVote = userVote.VideoReactionAuthorIdToVote.Single();
            short? vote = authorIdToVote.Value;
            var hashKey = $"f:{userVote.FixtureId}.t:{userVote.TeamId}.vra:{authorIdToVote.Key}.user-votes";
            var hashField = $"u:{userVote.UserId}";

            if (oldVote != null) {
                _transaction.AddCondition(Condition.HashEqual(hashKey, hashField, oldVote.Value));
            } else {
                _transaction.AddCondition(Condition.HashNotExists(hashKey, hashField));
            }

            if (vote != null) {
                _transaction.HashSetAsync(hashKey, hashField, vote.Value);
            } else {
                _transaction.HashDeleteAsync(hashKey, hashField);
            }
        }

        public void DeleteAllFor(
            long fixtureId, long teamId,
            List<string> fixtureParticipantKeys
        ) {
            _ensureTransaction();

            var keysToDelete = fixtureParticipantKeys.Select(
                participantKey => (RedisKey) $"f:{fixtureId}.t:{teamId}.{participantKey}.user-votes"
            );

            _transaction.KeyDeleteAsync(keysToDelete.ToArray());
        }
    }
}
