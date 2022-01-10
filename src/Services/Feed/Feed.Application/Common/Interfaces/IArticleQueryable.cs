using System.Collections.Generic;
using System.Threading.Tasks;

using Feed.Application.Article.Queries.Common.Dto;
using Feed.Application.Article.Queries.GetArticlesForTeam;

namespace Feed.Application.Common.Interfaces {
    public interface IArticleQueryable {
        Task<IEnumerable<ArticleWithUserVoteDto>> GetArticlesWithCommentCountFor(
            long teamId, ArticleFilter filter, int page
        );
        Task<ArticleWithUserVoteDto> GetArticleWithCommentCountById(long articleId);
    }
}
