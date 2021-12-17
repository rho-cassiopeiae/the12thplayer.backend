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

        public async Task<int> Create(Article article) {
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
                new NpgsqlParameter<int>(nameof(Article.Type), NpgsqlDbType.Integer) {
                    TypedValue = (int) article.Type
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
                new NpgsqlParameter<int>(nameof(Article.Rating), NpgsqlDbType.Integer) {
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

            var articleId = (int) await cmd.ExecuteScalarAsync();

            return articleId;
        }

        public async Task<int> UpdateRatingFor(int articleId, int incrementRatingBy) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter<int>(nameof(incrementRatingBy), NpgsqlDbType.Integer) {
                    TypedValue = incrementRatingBy
                },
                new NpgsqlParameter<int>(nameof(Article.Id), NpgsqlDbType.Integer) {
                    TypedValue = articleId
                }
            };

            cmd.Parameters.AddRange(parameters);

            int i = 0;
            cmd.CommandText = $@"
                UPDATE feed.""Articles""
                SET ""Rating"" = ""Rating"" + @{parameters[i++].ParameterName}
                WHERE ""Id"" = @{parameters[i++].ParameterName}
                RETURNING ""Rating"";
            ";

            var updatedRating = (int) await cmd.ExecuteScalarAsync();

            return updatedRating;
        }
    }
}
