using FluentValidation;

namespace Feed.Application.Article.Queries.GetArticlesForTeam {
    public class GetArticlesForTeamQueryValidator : AbstractValidator<GetArticlesForTeamQuery> {
        public GetArticlesForTeamQueryValidator() {
            RuleFor(q => q.TeamId).GreaterThan(0);
            RuleFor(q => q.Filter).IsInEnum();
            RuleFor(q => q.Page).GreaterThan(0);
        }
    }
}
