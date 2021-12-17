using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Npgsql;
using NpgsqlTypes;

using Feed.Application.Article.Queries.GetArticlesForTeam;
using Feed.Domain.Aggregates.Article;
using Feed.Application.Common.Interfaces;

namespace Feed.Infrastructure.Persistence.Queryables {
    public class ArticleQueryable : IArticleQueryable {
        private readonly FeedDbContext _feedDbContext;
        private readonly int _getCount;

        public ArticleQueryable(
            IConfiguration configuration,
            FeedDbContext feedDbContext
        ) {
            _feedDbContext = feedDbContext;
            _getCount = configuration.GetValue<int>("Article:GetCount");
        }

        public async Task<IEnumerable<ArticleDto>> GetArticlesWithCommentCountFor(
            long teamId, ArticleFilter filter, int page
        ) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            cmd.Parameters.Add(new NpgsqlParameter<long>(nameof(Article.TeamId), NpgsqlDbType.Bigint) {
                TypedValue = teamId
            });
            cmd.Parameters.Add(new NpgsqlParameter<int>("limit", NpgsqlDbType.Integer) {
                TypedValue = _getCount
            });
            cmd.Parameters.Add(new NpgsqlParameter<int>("offset", NpgsqlDbType.Integer) {
                TypedValue = (page - 1) * _getCount
            });

            int i = 0;
            string cte;
            switch (filter) {
                case ArticleFilter.Newest:
                    cte = $@"
                        WITH ""ArticleSummaries"" AS (
                            SELECT
                                ""Id"", ""AuthorId"", ""AuthorUsername"", ""PostedAt"", ""Type"",
                                ""Title"", ""PreviewImageUrl"", ""Summary"", ""Rating""
                            FROM feed.""Articles""
                            WHERE ""TeamId"" = @{cmd.Parameters[i++].ParameterName}
                            ORDER BY ""PostedAt"" DESC
                            LIMIT @{cmd.Parameters[i++].ParameterName}
                            OFFSET @{cmd.Parameters[i++].ParameterName}
                        )
                    ";
                    break;
                default:
                    cmd.Parameters.Insert(1, new NpgsqlParameter<long>(nameof(Article.PostedAt), NpgsqlDbType.Bigint) {
                        TypedValue = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(1)).ToUnixTimeMilliseconds()
                    });

                    cte = $@"
                        WITH ""ArticleSummaries"" AS (
                            SELECT
                                ""Id"", ""AuthorId"", ""AuthorUsername"", ""PostedAt"", ""Type"",
                                ""Title"", ""PreviewImageUrl"", ""Summary"", ""Rating""
                            FROM feed.""Articles""
                            WHERE
                                ""TeamId"" = @{cmd.Parameters[i++].ParameterName} AND
                                ""PostedAt"" > @{cmd.Parameters[i++].ParameterName}
                            ORDER BY ""Rating"" DESC
                            LIMIT @{cmd.Parameters[i++].ParameterName}
                            OFFSET @{cmd.Parameters[i++].ParameterName}
                        )
                    ";
                    break;
            }

            cmd.CommandText = $@"
                {cte},
                ""ArticleCommentCount"" (""ArticleId"", ""CommentCount"") AS (
                    SELECT a.""Id"", COUNT(a.""Id"")
                    FROM
                        ""ArticleSummaries"" AS a
                            INNER JOIN
                        feed.""Comments"" AS c
                            ON (a.""Id"" = c.""ArticleId"")
                    GROUP BY a.""Id""
                )

                SELECT
                    a.""Id"", a.""AuthorId"", a.""AuthorUsername"", a.""PostedAt"", a.""Type"",
                    a.""Title"", a.""PreviewImageUrl"", a.""Summary"", a.""Rating"", ac.""CommentCount""
                FROM
                    ""ArticleSummaries"" AS a
                        LEFT JOIN
                    ""ArticleCommentCount"" AS ac
                        ON (a.""Id"" = ac.""ArticleId"");
            ";

            var articles = new List<ArticleDto>();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                i = 0;
                articles.Add(new ArticleDto {
                    Id = reader.GetInt64(i),
                    AuthorId = reader.GetInt64(++i),
                    AuthorUsername = reader.GetString(++i),
                    PostedAt = reader.GetInt64(++i),
                    Type = reader.GetInt16(++i),
                    Title = reader.GetString(++i),
                    PreviewImageUrl = !reader.IsDBNull(++i) ? reader.GetString(i) : null,
                    Summary = !reader.IsDBNull(++i) ? reader.GetString(i) : null,
                    Rating = reader.GetInt64(++i),
                    CommentCount = !reader.IsDBNull(++i) ? reader.GetInt32(i) : 0
                });
            }

            return articles;
        }
    }
}
