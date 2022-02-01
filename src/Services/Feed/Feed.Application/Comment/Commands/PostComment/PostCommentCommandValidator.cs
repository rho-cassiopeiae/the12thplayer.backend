using System;

using FluentValidation;

namespace Feed.Application.Comment.Commands.PostComment {
    public class PostCommentCommandValidator : AbstractValidator<PostCommentCommand> {
        public PostCommentCommandValidator() {
            RuleFor(c => c.ArticleId).GreaterThan(0);
            RuleFor(c => c.ThreadRootCommentId).Must(value => value == null || Ulid.TryParse(value, out _));
            RuleFor(c => c.ParentCommentId).Must(value => value == null || Ulid.TryParse(value, out _));
            RuleFor(c => c.Body).NotEmpty().MaximumLength(500); // @@TODO: Config.
        }
    }
}
