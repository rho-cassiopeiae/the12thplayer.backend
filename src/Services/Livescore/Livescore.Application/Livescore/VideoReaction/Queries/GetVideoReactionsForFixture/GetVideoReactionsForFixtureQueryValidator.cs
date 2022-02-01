using FluentValidation;

namespace Livescore.Application.Livescore.VideoReaction.Queries.GetVideoReactionsForFixture {
    public class GetVideoReactionsForFixtureQueryValidator : AbstractValidator<GetVideoReactionsForFixtureQuery> {
        public GetVideoReactionsForFixtureQueryValidator() {
            RuleFor(q => q.FixtureId).GreaterThan(0);
            RuleFor(q => q.TeamId).GreaterThan(0);
            RuleFor(q => q.Filter).IsInEnum();
            RuleFor(q => q.Page).GreaterThan(0);
        }
    }
}
