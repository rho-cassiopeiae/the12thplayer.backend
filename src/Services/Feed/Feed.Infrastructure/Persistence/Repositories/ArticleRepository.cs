using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Npgsql;
using NpgsqlTypes;

using Feed.Domain.Aggregates.Article;

namespace Feed.Infrastructure.Persistence.Repositories {
    public class ArticleRepository : IArticleRepository {
        private readonly FeedDbContext _feedDbContext;

        public ArticleRepository(FeedDbContext feedDbContext) {
            _feedDbContext = feedDbContext;
        }

        public Task SaveChanges(CancellationToken cancellationToken) => Task.CompletedTask;

        public async Task<long> Create(Article article) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter<long>(nameof(Article.TeamId), NpgsqlDbType.Bigint) {
                    TypedValue = article.TeamId
                },
                new NpgsqlParameter<long>(nameof(Article.AuthorId), NpgsqlDbType.Bigint) {
                    TypedValue = article.AuthorId
                },
                new NpgsqlParameter<string>(nameof(Article.AuthorUsername), NpgsqlDbType.Text) {
                    TypedValue = article.AuthorUsername
                },
                new NpgsqlParameter<long>(nameof(Article.PostedAt), NpgsqlDbType.Bigint) {
                    TypedValue = article.PostedAt
                },
                new NpgsqlParameter<short>(nameof(Article.Type), NpgsqlDbType.Smallint) {
                    TypedValue = (short) article.Type
                },
                new NpgsqlParameter<string>(nameof(Article.Title), NpgsqlDbType.Text) {
                    TypedValue = article.Title
                },
                new NpgsqlParameter<string>(nameof(Article.PreviewImageUrl), NpgsqlDbType.Text) {
                    TypedValue = article.PreviewImageUrl
                },
                new NpgsqlParameter<string>(nameof(Article.Summary), NpgsqlDbType.Text) {
                    TypedValue = article.Summary
                },
                new NpgsqlParameter<string>(nameof(Article.Content), NpgsqlDbType.Text) {
                    TypedValue = article.Content
                },
                new NpgsqlParameter<long>(nameof(Article.Rating), NpgsqlDbType.Bigint) {
                    TypedValue = article.Rating
                }
            };

            cmd.Parameters.AddRange(parameters);

            int i = 0;
            cmd.CommandText = $@"
                INSERT INTO feed.""Articles"" (
                    ""TeamId"", ""AuthorId"", ""AuthorUsername"", ""PostedAt"", ""Type"",
                    ""Title"", ""PreviewImageUrl"", ""Summary"", ""Content"", ""Rating""
                )
                VALUES (
                    @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName},
                    @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName},
                    @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName},
                    @{parameters[i++].ParameterName}
                )
                RETURNING ""Id"";
            ";

            var articleId = (long) await cmd.ExecuteScalarAsync();

            return articleId;
        }

        public async Task<long> UpdateRatingFor(long articleId, int incrementRatingBy) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter<int>(nameof(incrementRatingBy), NpgsqlDbType.Integer) {
                    TypedValue = incrementRatingBy
                },
                new NpgsqlParameter<long>(nameof(Article.Id), NpgsqlDbType.Bigint) {
                    TypedValue = articleId
                }
            };

            cmd.Parameters.AddRange(parameters);

            cmd.CommandText = $@"
                UPDATE feed.""Articles""
                SET ""Rating"" = ""Rating"" + @{parameters.First().ParameterName}
                WHERE ""Id"" = @{parameters.Last().ParameterName}
                RETURNING ""Rating"";
            ";

            var updatedRating = (long) await cmd.ExecuteScalarAsync();

            return updatedRating;
        }
    }
}
