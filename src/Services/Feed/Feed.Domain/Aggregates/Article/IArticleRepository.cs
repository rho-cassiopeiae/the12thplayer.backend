using System.Threading.Tasks;

using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.Article {
    public interface IArticleRepository : IRepository<Article> {
        Task<int> Create(Article article);
        Task<int> UpdateRatingFor(int articleId, int incrementRatingBy);
    }
}
