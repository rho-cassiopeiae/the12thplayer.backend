using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Npgsql;
using NpgsqlTypes;

using Feed.Application.Comment.Queries.GetCommentsForArticle;
using Feed.Application.Common.Interfaces;
using Feed.Domain.Aggregates.Comment;

namespace Feed.Infrastructure.Persistence.Queryables {
    public class CommentQueryable : ICommentQueryable {
        private readonly FeedDbContext _feedDbContext;

        public CommentQueryable(FeedDbContext feedDbContext) {
            _feedDbContext = feedDbContext;
        }

        public async Task<IEnumerable<CommentWithUserVoteDto>> GetCommentsFor(long articleId) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            cmd.Parameters.Add(
                new NpgsqlParameter<long>(nameof(Comment.ArticleId), NpgsqlDbType.Bigint) {
                    TypedValue = articleId
                }
            );

            cmd.CommandText = $@"
                SELECT ""Id"", ""RootId"", ""ParentId"", ""AuthorId"", ""AuthorUsername"", ""Rating"", ""Body""
                FROM feed.""Comments""
                WHERE ""ArticleId"" = @{cmd.Parameters.First().ParameterName};
            ";

            var comments = new List<CommentWithUserVoteDto>();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                int i = 0;
                comments.Add(new CommentWithUserVoteDto {
                    Id = reader.GetString(i),
                    RootId = reader.GetString(++i),
                    ParentId = !reader.IsDBNull(++i) ? reader.GetString(i) : null,
                    AuthorId = reader.GetInt64(++i),
                    AuthorUsername = reader.GetString(++i),
                    Rating = reader.GetInt64(++i),
                    Body = reader.GetString(++i)
                });
            }

            return comments;
        }
    }
}
