using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.SignalR;

using MediatR;

using Feed.Application.Article.Commands.PostArticle;
using Feed.Application.Common.Results;
using Feed.Api.Hubs.Filters;
using Feed.Application.Article.Queries.GetArticlesForTeam;
using Feed.Application.Article.Queries.Common.Dto;
using Feed.Application.Article.Queries.GetArticle;
using Feed.Application.Article.Commands.VoteForArticle;

namespace Feed.Api.Hubs {
    public partial class FeedHub : Hub {
        private readonly ISender _mediator;

        public FeedHub(ISender mediator) {
            _mediator = mediator;
        }

        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<HandleResult<long>> PostArticle(PostArticleCommand command) => _mediator.Send(command);

        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<HandleResult<IEnumerable<ArticleWithUserVoteDto>>> GetArticlesForTeam(
            GetArticlesForTeamQuery query
        ) => _mediator.Send(query);

        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<HandleResult<ArticleWithUserVoteDto>> GetArticle(GetArticleQuery query) => _mediator.Send(query);

        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<HandleResult<ArticleRatingDto>> VoteForArticle(VoteForArticleCommand command) => _mediator.Send(command);
    }
}
