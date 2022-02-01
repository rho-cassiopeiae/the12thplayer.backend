using FluentValidation;

namespace Livescore.Application.Team.Queries.GetTeamSquad {
    public class GetTeamSquadQueryValidator : AbstractValidator<GetTeamSquadQuery> {
        public GetTeamSquadQueryValidator() {
            RuleFor(q => q.TeamId).GreaterThan(0);
        }
    }
}
