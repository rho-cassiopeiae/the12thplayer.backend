using System;
using System.Threading;
using System.Threading.Tasks;

using Npgsql;
using NpgsqlTypes;

using Feed.Domain.Aggregates.UserVote;
using Feed.Domain.Base;

namespace Feed.Infrastructure.Persistence.Repositories {
    public class UserVoteRepository : IUserVoteRepository {
        private readonly FeedDbContext _feedDbContext;

        private IUnitOfWork _unitOfWork;

        public UserVoteRepository(FeedDbContext feedDbContext) {
            _feedDbContext = feedDbContext;
        }

        public void EnlistAsPartOf(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _feedDbContext.Database.SetDbConnection(unitOfWork.Connection);
            _feedDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public Task SaveChanges(CancellationToken cancellationToken) => Task.CompletedTask;

        public async Task<UserVote> UpdateOneAndGetOldForArticle(long userId, long articleId, short vote) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter<long>(nameof(UserVote.UserId), NpgsqlDbType.Bigint) {
                    TypedValue = userId
                },
                new NpgsqlParameter<long>(nameof(UserVote.ArticleId), NpgsqlDbType.Bigint) {
                    TypedValue = articleId
                },
                new NpgsqlParameter<short>(nameof(UserVote.ArticleVote), NpgsqlDbType.Smallint) {
                    TypedValue = vote
                }
            };

            cmd.Parameters.AddRange(parameters);

            int i = 0;
            cmd.CommandText = $@"
                INSERT INTO feed.""UserVotes"" (""UserId"", ""ArticleId"", ""ArticleVote"")
                VALUES (@{parameters[i++].ParameterName}, @{parameters[i++].ParameterName}, @{parameters[i].ParameterName})
                ON CONFLICT (""UserId"", ""ArticleId"") DO
                    UPDATE
                    SET
                        ""ArticleVote"" =
                            CASE
                                WHEN
                                    ""UserVotes"".""ArticleVote"" = @{parameters[i].ParameterName}
                                THEN
                                    NULL
                                ELSE
                                    @{parameters[i].ParameterName}
                            END,
                        ""OldVote"" = ""UserVotes"".""ArticleVote""
                RETURNING ""OldVote"";
            ";

            var result = await cmd.ExecuteScalarAsync();

            return new UserVote(
                userId: userId,
                articleId: articleId,
                articleVote: result is DBNull ? null : (short) result
            );
        }
    }
}
