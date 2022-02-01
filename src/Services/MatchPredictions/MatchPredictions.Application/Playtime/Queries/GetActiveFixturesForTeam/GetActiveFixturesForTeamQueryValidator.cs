using FluentValidation;

namespace MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam {
    public class GetActiveFixturesForTeamQueryValidator : AbstractValidator<GetActiveFixturesForTeamQuery> {
        public GetActiveFixturesForTeamQueryValidator() {
            RuleFor(q => q.TeamId).GreaterThan(0);
        }
    }
}
