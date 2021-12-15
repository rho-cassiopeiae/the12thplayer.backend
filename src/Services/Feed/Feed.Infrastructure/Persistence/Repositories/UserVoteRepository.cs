using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;
using MediatR;

using Feed.Domain.Aggregates.UserVote;
using Feed.Domain.Base;

namespace Feed.Infrastructure.Persistence.Repositories {
    public class UserVoteRepository : IUserVoteRepository {
        private readonly FeedDbContext _feedDbContext;
        private readonly IPublisher _mediator;

        private IUnitOfWork _unitOfWork;

        public UserVoteRepository(
            FeedDbContext feedDbContext,
            IPublisher mediator
        ) {
            _feedDbContext = feedDbContext;
            _mediator = mediator;
        }

        public void EnlistAsPartOf(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _feedDbContext.Database.SetDbConnection(unitOfWork.Connection);
            _feedDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            var entities = _feedDbContext.ChangeTracker
                .Entries<Entity>()
                .Select(entry => entry.Entity)
                .Where(entity => entity.DomainEvents != null && entity.DomainEvents.Any())
                .ToList();

            var events = entities
                .SelectMany(entity => entity.DomainEvents)
                .ToList();

            foreach (var entity in entities) {
                entity.ClearDomainEvents();
            }

            foreach (var @event in events) {
                await _mediator.Publish(@event);
            }

            await _feedDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserVote> UpdateOneAndGetOldForArticle(long userId, int articleId, short vote) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = (NpgsqlConnection) _feedDbContext.Database.GetDbConnection();

            cmd.Parameters.Add(new NpgsqlParameter<long>(nameof(UserVote.UserId), NpgsqlDbType.Bigint) {
                TypedValue = userId
            });
            cmd.Parameters.Add(new NpgsqlParameter<int>(nameof(UserVote.ArticleId), NpgsqlDbType.Integer) {
                TypedValue = articleId
            });
            cmd.Parameters.Add(new NpgsqlParameter<short>(nameof(UserVote.ArticleVote), NpgsqlDbType.Smallint) {
                TypedValue = vote
            });

            int i = 0;
            cmd.CommandText = $@"
                SELECT * FROM feed.update_and_get_old_user_vote_for_article (
                    @{cmd.Parameters[i++].ParameterName},
                    @{cmd.Parameters[i++].ParameterName},
                    @{cmd.Parameters[i].ParameterName}
                );
            ";

            var oldVote = await cmd.ExecuteScalarAsync();

            var userVote = new UserVote(
                userId: userId,
                articleId: articleId,
                articleVote: oldVote is DBNull ? null : (short) oldVote
            );

            _feedDbContext.Attach(userVote);

            return userVote;
        }
    }
}
