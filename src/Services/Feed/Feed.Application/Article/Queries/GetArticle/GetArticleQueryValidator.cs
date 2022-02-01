using FluentValidation;

namespace Feed.Application.Article.Queries.GetArticle {
    public class GetArticleQueryValidator : AbstractValidator<GetArticleQuery> {
        public GetArticleQueryValidator() {
            RuleFor(q => q.ArticleId).GreaterThan(0);
        }
    }
}
