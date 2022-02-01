using FluentValidation;

namespace Livescore.Application.Livescore.PlayerRating.Queries.GetPlayerRatingsForFixture {
    public class GetPlayerRatingsForFixtureQueryValidator : AbstractValidator<GetPlayerRatingsForFixtureQuery> {
        public GetPlayerRatingsForFixtureQueryValidator() {
            RuleFor(q => q.FixtureId).GreaterThan(0);
            RuleFor(q => q.TeamId).GreaterThan(0);
        }
    }
}
