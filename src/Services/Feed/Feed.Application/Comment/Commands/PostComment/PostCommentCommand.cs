using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Common.Results;
using Feed.Domain.Aggregates.Comment;
using CommentDm = Feed.Domain.Aggregates.Comment.Comment;

namespace Feed.Application.Comment.Commands.PostComment {
    public class PostCommentCommand : IRequest<HandleResult<string>> {
        public long ArticleId { get; set; }
        public string ThreadRootCommentId { get; set; }
        public string ParentCommentId { get; set; }
        public string Body { get; set; }
    }

    public class PostCommentCommandHandler : IRequestHandler<PostCommentCommand, HandleResult<string>> {
        private readonly ICommentRepository _commentRepository;

        public PostCommentCommandHandler(ICommentRepository commentRepository) {
            _commentRepository = commentRepository;
        }

        public async Task<HandleResult<string>> Handle(
            PostCommentCommand command, CancellationToken cancellationToken
        ) {
            long userId = 1;
            var username = "user-1";

            var commentId = Ulid.NewUlid().ToString();
            var comment = new CommentDm(
                articleId: command.ArticleId,
                id: commentId,
                rootId: command.ThreadRootCommentId ?? commentId,
                parentId: command.ParentCommentId,
                authorId: userId,
                authorUsername: username,
                rating: 0,
                body: command.Body
            );

            await _commentRepository.Create(comment);

            return new HandleResult<string> {
                Data = comment.Id
            };
        }
    }
}
