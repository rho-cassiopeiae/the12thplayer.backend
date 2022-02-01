using FluentValidation;

namespace Feed.Application.Article.Commands.VoteForArticle {
    public class VoteForArticleCommandValidator : AbstractValidator<VoteForArticleCommand> {
        public VoteForArticleCommandValidator() {
            RuleFor(c => c.ArticleId).GreaterThan(0);
            RuleFor(c => c.UserVote).Must(value => value is null or 1 or -1);
        }
    }
}
