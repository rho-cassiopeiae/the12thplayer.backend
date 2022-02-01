using FluentValidation;

namespace Livescore.Application.Livescore.Fixture.Queries.GetFixtureForTeam {
    public class GetFixtureForTeamQueryValidator : AbstractValidator<GetFixtureForTeamQuery> {
        public GetFixtureForTeamQueryValidator() {
            RuleFor(q => q.FixtureId).GreaterThan(0);
            RuleFor(q => q.TeamId).GreaterThan(0);
        }
    }
}
