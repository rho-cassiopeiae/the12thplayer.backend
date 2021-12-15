using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;

using Feed.Domain.Aggregates.Article;

namespace Feed.Infrastructure.Persistence.Repositories {
    public class ArticleRepository : IArticleRepository {
        private readonly FeedDbContext _feedDbContext;

        public ArticleRepository(FeedDbContext feedDbContext) {
            _feedDbContext = feedDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _feedDbContext.SaveChangesAsync(cancellationToken);
        }

        public void Create(Article article) {
            _feedDbContext.Articles.Add(article);
        }

        public async Task<int> UpdateRatingFor(int articleId, int incrementRatingBy) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = (NpgsqlConnection) _feedDbContext.Database.GetDbConnection();

            cmd.Parameters.Add(new NpgsqlParameter<int>(nameof(Article.Id), NpgsqlDbType.Integer) {
                TypedValue = articleId
            });
            cmd.Parameters.Add(new NpgsqlParameter<int>(nameof(incrementRatingBy), NpgsqlDbType.Integer) {
                TypedValue = incrementRatingBy
            });

            int i = 0;
            cmd.CommandText = $@"
                SELECT * FROM feed.update_article_rating (
                    @{cmd.Parameters[i++].ParameterName},
                    @{cmd.Parameters[i].ParameterName}
                );
            ";

            var updatedRating = (int) await cmd.ExecuteScalarAsync();

            return updatedRating;
        }
    }
}
