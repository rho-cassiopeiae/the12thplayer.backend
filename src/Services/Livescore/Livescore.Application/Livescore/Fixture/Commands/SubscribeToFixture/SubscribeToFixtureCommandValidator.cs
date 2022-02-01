using FluentValidation;

namespace Livescore.Application.Livescore.Fixture.Commands.SubscribeToFixture {
    public class SubscribeToFixtureCommandValidator : AbstractValidator<SubscribeToFixtureCommand> {
        public SubscribeToFixtureCommandValidator() {
            RuleFor(c => c.FixtureId).GreaterThan(0);
            RuleFor(c => c.TeamId).GreaterThan(0);
        }
    }
}
