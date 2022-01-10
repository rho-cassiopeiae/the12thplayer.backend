using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Common.Interfaces;
using Feed.Application.Common.Results;

namespace Feed.Application.Comment.Queries.GetCommentsForArticle {
    public class GetCommentsForArticleQuery : IRequest<HandleResult<IEnumerable<CommentWithUserVoteDto>>> {
        public long ArticleId { get; set; }
    }

    public class GetCommentsForArticleQueryHandler : IRequestHandler<
        GetCommentsForArticleQuery, HandleResult<IEnumerable<CommentWithUserVoteDto>>
    > {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly ICommentQueryable _commentQueryable;
        private readonly IUserVoteQueryable _userVoteQueryable;

        public GetCommentsForArticleQueryHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            ICommentQueryable commentQueryable,
            IUserVoteQueryable userVoteQueryable
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _commentQueryable = commentQueryable;
            _userVoteQueryable = userVoteQueryable;
        }

        public async Task<HandleResult<IEnumerable<CommentWithUserVoteDto>>> Handle(
            GetCommentsForArticleQuery query, CancellationToken cancellationToken
        ) {
            var comments = await _commentQueryable.GetCommentsFor(query.ArticleId);

            if (comments.Any() && _authenticationContext.User != null) {
                long userId = _principalDataProvider.GetId(_authenticationContext.User);

                var userVote = await _userVoteQueryable.GetCommentVotesFor(userId, query.ArticleId);
                if (userVote != null) {
                    foreach (var commentIdToVote in userVote.CommentIdToVote) {
                        var comment = comments.FirstOrDefault(c => c.Id == commentIdToVote.Key);
                        if (comment != null) { // @@NOTE: Checking that comment hasn't been removed.
                            comment.UserVote = commentIdToVote.Value.Value;
                        }
                    }
                }
            }

            return new HandleResult<IEnumerable<CommentWithUserVoteDto>> {
                Data = comments
            };
        }
    }
}
