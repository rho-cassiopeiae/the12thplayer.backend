using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Article.Queries.Common.Dto;
using Feed.Application.Common.Results;
using Feed.Application.Common.Interfaces;
using Feed.Application.Article.Common.Errors;

namespace Feed.Application.Article.Queries.GetArticle {
    public class GetArticleQuery : IRequest<HandleResult<ArticleWithUserVoteDto>> {
        public long ArticleId { get; set; }
    }

    public class GetArticleQueryHandler : IRequestHandler<GetArticleQuery, HandleResult<ArticleWithUserVoteDto>> {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IArticleQueryable _articleQueryable;
        private readonly IUserVoteQueryable _userVoteQueryable;

        public GetArticleQueryHandler(
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

        public async Task<HandleResult<ArticleWithUserVoteDto>> Handle(
            GetArticleQuery query, CancellationToken cancellationToken
        ) {
            var article = await _articleQueryable.GetArticleWithCommentCountById(query.ArticleId);
            if (article == null) {
                return new HandleResult<ArticleWithUserVoteDto> {
                    Error = new ArticleError("Specified article does not exist")
                };
            }

            if (_authenticationContext.User != null) {
                long userId = _principalDataProvider.GetId(_authenticationContext.User);

                var userVote = await _userVoteQueryable.GetArticleVoteFor(userId, article.Id);
                article.UserVote = userVote?.ArticleVote.Value;
            }

            return new HandleResult<ArticleWithUserVoteDto> {
                Data = article
            };
        }
    }
}
