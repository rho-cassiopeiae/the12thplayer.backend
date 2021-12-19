using System.Threading;
using System.Threading.Tasks;

using Npgsql;
using NpgsqlTypes;

using Feed.Domain.Aggregates.Comment;

namespace Feed.Infrastructure.Persistence.Repositories {
    public class CommentRepository : ICommentRepository {
        private readonly FeedDbContext _feedDbContext;

        public CommentRepository(FeedDbContext feedDbContext) {
            _feedDbContext = feedDbContext;
        }

        public Task SaveChanges(CancellationToken cancellationToken) => Task.CompletedTask;

        public async Task Create(Comment comment) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter<long>(nameof(Comment.ArticleId), NpgsqlDbType.Bigint) {
                    TypedValue = comment.ArticleId
                },
                new NpgsqlParameter<string>(nameof(Comment.Id), NpgsqlDbType.Text) {
                    TypedValue = comment.Id
                },
                new NpgsqlParameter<string>(nameof(Comment.RootId), NpgsqlDbType.Text) {
                    TypedValue = comment.RootId
                },
                new NpgsqlParameter<string>(nameof(Comment.ParentId), NpgsqlDbType.Text) {
                    TypedValue = comment.ParentId
                },
                new NpgsqlParameter<long>(nameof(Comment.AuthorId), NpgsqlDbType.Bigint) {
                    TypedValue = comment.AuthorId
                },
                new NpgsqlParameter<string>(nameof(Comment.AuthorUsername), NpgsqlDbType.Text) {
                    TypedValue = comment.AuthorUsername
                },
                new NpgsqlParameter<long>(nameof(Comment.Rating), NpgsqlDbType.Bigint) {
                    TypedValue = comment.Rating
                },
                new NpgsqlParameter<string>(nameof(Comment.Body), NpgsqlDbType.Text) {
                    TypedValue = comment.Body
                }
            };

            cmd.Parameters.AddRange(parameters);

            int i = 0;
            cmd.CommandText = $@"
                INSERT INTO feed.""Comments"" (
                    ""ArticleId"", ""Id"", ""RootId"", ""ParentId"", ""AuthorId"", ""AuthorUsername"", ""Rating"", ""Body""
                )
                VALUES (
                    @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName},
                    @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName},
                    @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName}
                );
            ";

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<long> UpdateRatingFor(long articleId, string commentId, int incrementRatingBy) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter<int>(nameof(incrementRatingBy), NpgsqlDbType.Integer) {
                    TypedValue = incrementRatingBy
                },
                new NpgsqlParameter<long>(nameof(Comment.ArticleId), NpgsqlDbType.Bigint) {
                    TypedValue = articleId
                },
                new NpgsqlParameter<string>(nameof(Comment.Id), NpgsqlDbType.Text) {
                    TypedValue = commentId
                }
            };

            cmd.Parameters.AddRange(parameters);

            int i = 0;
            cmd.CommandText = $@"
                UPDATE feed.""Comments""
                SET ""Rating"" = ""Rating"" + @{parameters[i++].ParameterName}
                WHERE
                    ""ArticleId"" = @{parameters[i++].ParameterName} AND
                    ""Id"" = @{parameters[i++].ParameterName}
                RETURNING ""Rating"";
            ";

            var updatedRating = (long) await cmd.ExecuteScalarAsync();

            return updatedRating;
        }
    }
}
