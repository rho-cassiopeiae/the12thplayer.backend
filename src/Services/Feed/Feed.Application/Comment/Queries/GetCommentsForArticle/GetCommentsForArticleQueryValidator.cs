using FluentValidation;

namespace Feed.Application.Comment.Queries.GetCommentsForArticle {
    public class GetCommentsForArticleQueryValidator : AbstractValidator<GetCommentsForArticleQuery> {
        public GetCommentsForArticleQueryValidator() {
            RuleFor(q => q.ArticleId).GreaterThan(0);
        }
    }
}
