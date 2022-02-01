using System;

using FluentValidation;

namespace Feed.Application.Comment.Commands.VoteForComment {
    public class VoteForCommentCommandValidator : AbstractValidator<VoteForCommentCommand> {
        public VoteForCommentCommandValidator() {
            RuleFor(c => c.ArticleId).GreaterThan(0);
            RuleFor(c => c.CommentId).Must(value => Ulid.TryParse(value, out _));
            RuleFor(c => c.UserVote).Must(value => value is null or 1 or -1);
        }
    }
}
