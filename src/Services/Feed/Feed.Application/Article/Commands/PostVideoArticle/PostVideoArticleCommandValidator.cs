using FluentValidation;

namespace Feed.Application.Article.Commands.PostVideoArticle {
    public class PostVideoArticleCommandValidator : AbstractValidator<PostVideoArticleCommand> {
        public PostVideoArticleCommandValidator() {
            RuleFor(c => c.TeamId).GreaterThan(0);
        }
    }
}
