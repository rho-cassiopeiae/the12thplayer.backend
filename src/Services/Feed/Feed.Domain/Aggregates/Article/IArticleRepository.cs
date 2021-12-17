using System.Threading.Tasks;

using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.Article {
    public interface IArticleRepository : IRepository<Article> {
        Task<long> Create(Article article);
        Task<long> UpdateRatingFor(long articleId, int incrementRatingBy);
    }
}
