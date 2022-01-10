using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Npgsql;
using NpgsqlTypes;

using Feed.Application.Article.Queries.GetArticlesForTeam;
using Feed.Domain.Aggregates.Article;
using Feed.Application.Common.Interfaces;
using Feed.Application.Article.Queries.Common.Dto;

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

        public async Task<IEnumerable<ArticleWithUserVoteDto>> GetArticlesWithCommentCountFor(
            long teamId, ArticleFilter filter, int page
        ) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            cmd.Parameters.Add(
                new NpgsqlParameter<long>(nameof(Article.TeamId), NpgsqlDbType.Bigint) {
                    TypedValue = teamId
                }
            );
            cmd.Parameters.Add(
                new NpgsqlParameter<int>("limit", NpgsqlDbType.Integer) {
                    TypedValue = _getCount
                }
            );
            cmd.Parameters.Add(
                new NpgsqlParameter<int>("offset", NpgsqlDbType.Integer) {
                    TypedValue = (page - 1) * _getCount
                }
            );

            int i = 0;
            string cte;
            string orderBy;
            switch (filter) {
                case ArticleFilter.Newest:
                    orderBy = @"""PostedAt"" DESC";
                    cte = $@"
                        WITH ""ArticleSummaries"" AS (
                            SELECT
                                ""Id"", ""AuthorId"", ""AuthorUsername"", ""PostedAt"", ""Type"",
                                ""Title"", ""PreviewImageUrl"", ""Summary"", ""Rating"",
                                CASE
                                    WHEN
                                        ""Type"" IN ({(short) ArticleType.Highlights}, {(short) ArticleType.Video})
                                    THEN
                                        ""Content""
                                    ELSE
                                        NULL
                                END AS ""Content""
                            FROM feed.""Articles""
                            WHERE ""TeamId"" = @{cmd.Parameters[i++].ParameterName}
                            ORDER BY {orderBy}
                            LIMIT @{cmd.Parameters[i++].ParameterName}
                            OFFSET @{cmd.Parameters[i++].ParameterName}
                        )
                    ";
                    break;
                default:
                    orderBy = @"""Rating"" DESC";
                    cmd.Parameters.Insert(
                        1,
                        new NpgsqlParameter<long>(nameof(Article.PostedAt), NpgsqlDbType.Bigint) {
                            TypedValue = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(1)).ToUnixTimeMilliseconds()
                        }
                    );

                    cte = $@"
                        WITH ""ArticleSummaries"" AS (
                            SELECT
                                ""Id"", ""AuthorId"", ""AuthorUsername"", ""PostedAt"", ""Type"",
                                ""Title"", ""PreviewImageUrl"", ""Summary"", ""Rating"",
                                CASE
                                    WHEN
                                        ""Type"" IN ({(short) ArticleType.Highlights}, {(short) ArticleType.Video})
                                    THEN
                                        ""Content""
                                    ELSE
                                        NULL
                                END AS ""Content""
                            FROM feed.""Articles""
                            WHERE
                                ""TeamId"" = @{cmd.Parameters[i++].ParameterName} AND
                                ""PostedAt"" > @{cmd.Parameters[i++].ParameterName}
                            ORDER BY {orderBy}
                            LIMIT @{cmd.Parameters[i++].ParameterName}
                            OFFSET @{cmd.Parameters[i++].ParameterName}
                        )
                    ";
                    break;
            }

            cmd.CommandText = $@"
                {cte},
                ""ArticleCommentCount"" (""ArticleId"", ""CommentCount"") AS (
                    SELECT ""ArticleId"", COUNT(*)
                    FROM feed.""Comments""
                    WHERE ""ArticleId"" IN (
                        SELECT ""Id""
                        FROM ""ArticleSummaries""
                    )
                    GROUP BY ""ArticleId""
                )

                SELECT
                    a.""Id"", a.""AuthorId"", a.""AuthorUsername"", a.""PostedAt"", a.""Type"", a.""Title"",
                    a.""PreviewImageUrl"", a.""Summary"", a.""Content"", a.""Rating"", ac.""CommentCount""
                FROM
                    ""ArticleSummaries"" AS a
                        LEFT JOIN
                    ""ArticleCommentCount"" AS ac
                        ON (a.""Id"" = ac.""ArticleId"")
                ORDER BY a.{orderBy};
            ";

            var articles = new List<ArticleWithUserVoteDto>();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                i = 0;
                articles.Add(new ArticleWithUserVoteDto {
                    Id = reader.GetInt64(i),
                    AuthorId = reader.GetInt64(++i),
                    AuthorUsername = reader.GetString(++i),
                    PostedAt = reader.GetInt64(++i),
                    Type = reader.GetInt16(++i),
                    Title = reader.GetString(++i),
                    PreviewImageUrl = !reader.IsDBNull(++i) ? reader.GetString(i) : null,
                    Summary = !reader.IsDBNull(++i) ? reader.GetString(i) : null,
                    Content = !reader.IsDBNull(++i) ? reader.GetString(i) : null,
                    Rating = reader.GetInt64(++i),
                    CommentCount = !reader.IsDBNull(++i) ? reader.GetInt32(i) : 0
                });
            }

            return articles;
        }

        public async Task<ArticleWithUserVoteDto> GetArticleWithCommentCountById(long articleId) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            cmd.Parameters.Add(
                new NpgsqlParameter<long>(nameof(Article.Id), NpgsqlDbType.Bigint) {
                    TypedValue = articleId
                }
            );

            cmd.CommandText = $@"
                SELECT
                    a.""AuthorId"", a.""AuthorUsername"", a.""PostedAt"", a.""Type"", a.""Title"",
                    a.""PreviewImageUrl"", a.""Summary"", a.""Content"", a.""Rating"", COUNT(c.""Id"")
                FROM
                    feed.""Articles"" AS a
                        LEFT JOIN
                    feed.""Comments"" AS c
                        ON (a.""Id"" = c.""ArticleId"")
                WHERE a.""Id"" = @{cmd.Parameters.First().ParameterName};
            ";

            ArticleWithUserVoteDto article = null;

            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync()) {
                int i = 0;
                article = new ArticleWithUserVoteDto {
                    Id = articleId,
                    AuthorId = reader.GetInt64(i),
                    AuthorUsername = reader.GetString(++i),
                    PostedAt = reader.GetInt64(++i),
                    Type = reader.GetInt16(++i),
                    Title = reader.GetString(++i),
                    PreviewImageUrl = !reader.IsDBNull(++i) ? reader.GetString(i) : null,
                    Summary = !reader.IsDBNull(++i) ? reader.GetString(i) : null,
                    Content = reader.GetString(++i),
                    Rating = reader.GetInt64(++i),
                    CommentCount = reader.GetInt32(++i)
                };
            }

            return article;
        }
    }
}
