using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Common.Interfaces;
using Feed.Application.Common.Results;
using Feed.Application.Article.Queries.Common.Dto;

namespace Feed.Application.Article.Queries.GetArticlesForTeam {
    public class GetArticlesForTeamQuery : IRequest<HandleResult<IEnumerable<ArticleWithUserVoteDto>>> {
        public long TeamId { get; set; }
        public ArticleFilter Filter { get; set; }
        public int Page { get; set; }
    }

    public class GetArticlesForTeamQueryHandler : IRequestHandler<
        GetArticlesForTeamQuery, HandleResult<IEnumerable<ArticleWithUserVoteDto>>
    > {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IArticleQueryable _articleQueryable;
        private readonly IUserVoteQueryable _userVoteQueryable;

        public GetArticlesForTeamQueryHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IArticleQueryable articleQueryable,
            IUserVoteQueryable userVoteQueryable
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _articleQueryable = articleQueryable;
            _userVoteQueryable = userVoteQueryable;
        }

        public async Task<HandleResult<IEnumerable<ArticleWithUserVoteDto>>> Handle(
            GetArticlesForTeamQuery query, CancellationToken cancellationToken
        ) {
            var articles = await _articleQueryable.GetArticlesWithCommentCountFor(
                query.TeamId, query.Filter, query.Page
            );

            if (articles.Any() && _authenticationContext.User != null) {
                long userId = _principalDataProvider.GetId(_authenticationContext.User);

                var userVotes = await _userVoteQueryable.GetArticleVotesFor(userId, articles.Select(a => a.Id));
                foreach (var userVote in userVotes) {
                    var article = articles.First(a => a.Id == userVote.ArticleId);
                    article.UserVote = userVote.ArticleVote.Value;
                }
            }

            return new HandleResult<IEnumerable<ArticleWithUserVoteDto>> {
                Data = articles
            };
        }
    }
}
