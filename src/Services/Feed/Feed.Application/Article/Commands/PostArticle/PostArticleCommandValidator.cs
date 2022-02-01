using System;

using FluentValidation;

namespace Feed.Application.Article.Commands.PostArticle {
    public class PostArticleCommandValidator : AbstractValidator<PostArticleCommand> {
        public PostArticleCommandValidator() {
            RuleFor(c => c.TeamId).GreaterThan(0);
            RuleFor(c => c.Type).IsInEnum();
            RuleFor(c => c.Title).NotEmpty().MaximumLength(200); // @@TODO: Config.
            RuleFor(c => c.PreviewImageUrl).Must(value =>
                value == null || Uri.TryCreate(value, UriKind.Absolute, out _)
            );
            RuleFor(c => c.Summary).Must(value => value == null || value.Length <= 300); // @@TODO: Config.
            RuleFor(c => c.Content).NotEmpty();
        }
    }
}
