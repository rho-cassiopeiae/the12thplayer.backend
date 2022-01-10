using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Feed.Api.Hubs.Filters;
using Feed.Application.Comment.Queries.GetCommentsForArticle;
using Feed.Application.Common.Results;
using Feed.Application.Comment.Commands.PostComment;
using Feed.Application.Comment.Commands.VoteForComment;

namespace Feed.Api.Hubs {
    public partial class FeedHub : Hub {
        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<HandleResult<IEnumerable<CommentWithUserVoteDto>>> GetCommentsForArticle(
            GetCommentsForArticleQuery query
        ) => _mediator.Send(query);

        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<HandleResult<CommentRatingDto>> VoteForComment(VoteForCommentCommand command) => _mediator.Send(command);

        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<HandleResult<string>> PostComment(PostCommentCommand command) => _mediator.Send(command);
    }
}
