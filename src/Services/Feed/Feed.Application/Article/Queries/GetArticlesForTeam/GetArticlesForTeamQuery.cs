using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Common.Interfaces;
using Feed.Application.Common.Results;

namespace Feed.Application.Article.Queries.GetArticlesForTeam {
    public class GetArticlesForTeamQuery : IRequest<HandleResult<IEnumerable<ArticleDto>>> {
        public long TeamId { get; set; }
        public int Filter { get; set; }
        public int Page { get; set; }
    }

    public class GetArticlesForTeamQueryHandler : IRequestHandler<
        GetArticlesForTeamQuery, HandleResult<IEnumerable<ArticleDto>>
    > {
        private readonly IArticleQueryable _articleQueryable;

        public GetArticlesForTeamQueryHandler(IArticleQueryable articleQueryable) {
            _articleQueryable = articleQueryable;
        }

        public async Task<HandleResult<IEnumerable<ArticleDto>>> Handle(
            GetArticlesForTeamQuery query, CancellationToken cancellationToken
        ) {
            return new HandleResult<IEnumerable<ArticleDto>> {
                Data = await _articleQueryable.GetArticlesWithCommentCountFor(
                    query.TeamId, (ArticleFilter) query.Filter, query.Page
                )
            };
        }
    }
}
