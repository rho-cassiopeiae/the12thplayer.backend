using FluentValidation;

namespace Livescore.Application.Team.Queries.GetPlayerRatingsForParticipant {
    public class GetPlayerRatingsForParticipantQueryValidator : AbstractValidator<GetPlayerRatingsForParticipantQuery> {
        public GetPlayerRatingsForParticipantQueryValidator() {
            RuleFor(q => q.TeamId).GreaterThan(0);
            RuleForEach(q => q.ParticipantKeys).Must(value => {
                var valueSplit = value?.Split(':');
                return valueSplit != null &&
                    valueSplit.Length == 2 &&
                    valueSplit[0].Length == 1 && valueSplit[0][0] is 'm' or 'p' or 's' &&
                    long.TryParse(valueSplit[1], out long _);
            });
        }
    }
}
