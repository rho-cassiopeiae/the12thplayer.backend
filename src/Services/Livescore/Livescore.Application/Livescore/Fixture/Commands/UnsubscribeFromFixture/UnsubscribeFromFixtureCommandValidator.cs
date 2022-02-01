using FluentValidation;

namespace Livescore.Application.Livescore.Fixture.Commands.UnsubscribeFromFixture {
    public class UnsubscribeFromFixtureCommandValidator : AbstractValidator<UnsubscribeFromFixtureCommand> {
        public UnsubscribeFromFixtureCommandValidator() {
            RuleFor(c => c.FixtureId).GreaterThan(0);
            RuleFor(c => c.TeamId).GreaterThan(0);
        }
    }
}
