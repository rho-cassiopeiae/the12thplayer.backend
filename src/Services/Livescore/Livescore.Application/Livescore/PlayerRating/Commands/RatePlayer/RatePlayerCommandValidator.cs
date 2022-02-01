using FluentValidation;

namespace Livescore.Application.Livescore.PlayerRating.Commands.RatePlayer {
    public class RatePlayerCommandValidator : AbstractValidator<RatePlayerCommand> {
        public RatePlayerCommandValidator() {
            RuleFor(c => c.FixtureId).GreaterThan(0);
            RuleFor(c => c.TeamId).GreaterThan(0);
            RuleFor(c => c.ParticipantKey).Must(value => {
                var valueSplit = value?.Split(':');
                return valueSplit != null &&
                    valueSplit.Length == 2 &&
                    valueSplit[0].Length == 1 && valueSplit[0][0] is 'm' or 'p' or 's' &&
                    long.TryParse(valueSplit[1], out long _);
            });
            RuleFor(c => c.Rating).InclusiveBetween(0.0f, 10.0f);
        }
    }
}
