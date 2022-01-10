using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Npgsql;
using NpgsqlTypes;

using Feed.Domain.Aggregates.UserVote;
using Feed.Application.Common.Interfaces;

namespace Feed.Infrastructure.Persistence.Queryables {
    public class UserVoteQueryable : IUserVoteQueryable {
        private readonly FeedDbContext _feedDbContext;

        public UserVoteQueryable(FeedDbContext feedDbContext) {
            _feedDbContext = feedDbContext;
        }

        public async Task<IEnumerable<UserVote>> GetArticleVotesFor(long userId, IEnumerable<long> articleIds) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            cmd.Parameters.Add(
                new NpgsqlParameter<long>(nameof(UserVote.UserId), NpgsqlDbType.Bigint) {
                    TypedValue = userId
                }
            );
            cmd.Parameters.Add(
                new NpgsqlParameter<long[]>(nameof(UserVote.ArticleId), NpgsqlDbType.Array | NpgsqlDbType.Bigint) {
                    TypedValue = articleIds.ToArray()
                }
            );

            cmd.CommandText = $@"
                SELECT ""ArticleId"", ""ArticleVote""
                FROM feed.""UserVotes""
                WHERE
                    ""UserId"" = @{cmd.Parameters.First().ParameterName} AND
                    ""ArticleId"" = ANY(@{cmd.Parameters.Last().ParameterName}) AND
                    ""ArticleVote"" IS NOT NULL;
            ";

            var userVotes = new List<UserVote>();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                int i = 0;
                userVotes.Add(new UserVote(
                    userId: userId,
                    articleId: reader.GetInt64(i),
                    articleVote: reader.GetInt16(++i)
                ));
            }

            return userVotes;
        }

        public async Task<UserVote> GetArticleVoteFor(long userId, long articleId) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            cmd.Parameters.Add(
                new NpgsqlParameter<long>(nameof(UserVote.UserId), NpgsqlDbType.Bigint) {
                    TypedValue = userId
                }
            );
            cmd.Parameters.Add(
                new NpgsqlParameter<long>(nameof(UserVote.ArticleId), NpgsqlDbType.Bigint) {
                    TypedValue = articleId
                }
            );

            cmd.CommandText = $@"
                SELECT ""ArticleVote""
                FROM feed.""UserVotes""
                WHERE
                    ""UserId"" = @{cmd.Parameters.First().ParameterName} AND
                    ""ArticleId"" = @{cmd.Parameters.Last().ParameterName} AND
                    ""ArticleVote"" IS NOT NULL;
            ";

            UserVote userVote = null;

            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync()) {
                userVote = new UserVote(
                    userId: userId,
                    articleId: articleId,
                    articleVote: reader.GetInt16(0)
                );
            }

            return userVote;
        }

        public async Task<UserVote> GetCommentVotesFor(long userId, long articleId) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            cmd.Parameters.Add(
                new NpgsqlParameter<long>(nameof(UserVote.UserId), NpgsqlDbType.Bigint) {
                    TypedValue = userId
                }
            );
            cmd.Parameters.Add(
                new NpgsqlParameter<long>(nameof(UserVote.ArticleId), NpgsqlDbType.Bigint) {
                    TypedValue = articleId
                }
            );

            cmd.CommandText = $@"
                SELECT ""CommentIdToVote""
                FROM feed.""UserVotes""
                WHERE
                    ""UserId"" = @{cmd.Parameters.First().ParameterName} AND
                    ""ArticleId"" = @{cmd.Parameters.Last().ParameterName};
            ";

            UserVote userVote = null;

            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync()) {
                var commentVotes = reader.GetFieldValue<IDictionary<string, short?>>(0);
                userVote = new UserVote(
                    userId: userId,
                    articleId: articleId
                );
                userVote.SetCommentVotes(commentVotes);
            }

            return userVote;
        }
    }
}
