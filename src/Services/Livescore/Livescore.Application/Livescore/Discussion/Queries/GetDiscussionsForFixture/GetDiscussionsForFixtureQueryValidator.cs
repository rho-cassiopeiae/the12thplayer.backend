using FluentValidation;

namespace Livescore.Application.Livescore.Discussion.Queries.GetDiscussionsForFixture {
    public class GetDiscussionsForFixtureQueryValidator : AbstractValidator<GetDiscussionsForFixtureQuery> {
        public GetDiscussionsForFixtureQueryValidator() {
            RuleFor(q => q.FixtureId).GreaterThan(0);
            RuleFor(q => q.TeamId).GreaterThan(0);
        }
    }
}
