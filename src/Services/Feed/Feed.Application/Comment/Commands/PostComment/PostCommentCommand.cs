using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Common.Results;
using Feed.Application.Common.Attributes;
using Feed.Application.Common.Interfaces;
using Feed.Domain.Aggregates.Comment;
using CommentDm = Feed.Domain.Aggregates.Comment.Comment;

namespace Feed.Application.Comment.Commands.PostComment {
    [RequireAuthorization]
    public class PostCommentCommand : IRequest<HandleResult<string>> {
        public long ArticleId { get; set; }
        public string ThreadRootCommentId { get; set; }
        public string ParentCommentId { get; set; }
        public string Body { get; set; }
    }

    public class PostCommentCommandHandler : IRequestHandler<PostCommentCommand, HandleResult<string>> {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly ICommentRepository _commentRepository;

        public PostCommentCommandHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            ICommentRepository commentRepository
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _commentRepository = commentRepository;
        }

        public async Task<HandleResult<string>> Handle(
            PostCommentCommand command, CancellationToken cancellationToken
        ) {
            var commentId = Ulid.NewUlid().ToString();
            var comment = new CommentDm(
                articleId: command.ArticleId,
                id: commentId,
                rootId: command.ThreadRootCommentId ?? commentId,
                parentId: command.ParentCommentId,
                authorId: _principalDataProvider.GetId(_authenticationContext.User),
                authorUsername: _principalDataProvider.GetUsername(_authenticationContext.User),
                rating: 0,
                body: command.Body
            );

            // @@TODO: Deal with deleted root/parent comment.

            await _commentRepository.Create(comment);

            return new HandleResult<string> {
                Data = comment.Id
            };
        }
    }
}
