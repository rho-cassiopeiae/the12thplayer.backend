using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Npgsql;
using NpgsqlTypes;

using Feed.Application.Comment.Queries.GetCommentsForArticle;
using Feed.Domain.Aggregates.Comment;
using Feed.Application.Common.Interfaces;

namespace Feed.Infrastructure.Persistence.Queryables {
    public class CommentQueryable : ICommentQueryable {
        private readonly FeedDbContext _feedDbContext;

        private readonly int _getCount;

        public CommentQueryable(
            IConfiguration configuration,
            FeedDbContext feedDbContext
        ) {
            _feedDbContext = feedDbContext;
            _getCount = configuration.GetValue<int>("Comment:GetCount");
        }

        public async Task<IEnumerable<Comment>> GetCommentsFor(
            long articleId, CommentFilter filter, int page
        ) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            string orderByClause;
            switch (filter) {
                case CommentFilter.OldestFirst:
                    orderByClause = @"""Id"" ASC";
                    break;
                case CommentFilter.NewestFirst:
                    orderByClause = @"""Id"" DESC";
                    break;
                default:
                    orderByClause = @"""Rating"" DESC";
                    break;
            }

            var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter<long>(nameof(articleId), NpgsqlDbType.Bigint) {
                    TypedValue = articleId
                },
                new NpgsqlParameter<int>("limit", NpgsqlDbType.Integer) {
                    TypedValue = _getCount
                },
                new NpgsqlParameter<int>("offset", NpgsqlDbType.Integer) {
                    TypedValue = (page - 1) * _getCount
                }
            };

            cmd.Parameters.AddRange(parameters);

            int i = 0;
            cmd.CommandText = $@"
                WITH ""RootComments"" AS (
                    SELECT ""ArticleId"", ""Id""
                    FROM feed.""Comments""
                    WHERE
                        ""ArticleId"" = @{parameters[i++].ParameterName} AND
                        ""ParentId"" IS NULL
                    ORDER BY {orderByClause}
                    LIMIT @{parameters[i++].ParameterName}
                    OFFSET @{parameters[i++].ParameterName}
                )

                SELECT
                    c.""Id"", c.""RootId"", c.""ParentId"", c.""AuthorId"",
                    c.""AuthorUsername"", c.""Rating"", c.""Body""
                FROM
                    ""RootComments"" AS r
                        INNER JOIN
                    feed.""Comments"" AS c
                        ON (
                            r.""ArticleId"" = c.""ArticleId"" AND
                            r.""Id"" = c.""RootId""
                        )
            ";

            var comments = new List<Comment>();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                i = 0;
                comments.Add(new Comment(
                    articleId: articleId,
                    id: reader.GetString(i),
                    rootId: reader.GetString(++i),
                    parentId: !reader.IsDBNull(++i) ? reader.GetString(i) : null,
                    authorId: reader.GetInt64(++i),
                    authorUsername: reader.GetString(++i),
                    rating: reader.GetInt64(++i),
                    body: reader.GetString(++i)
                ));
            }

            return comments;
        }
    }
}
