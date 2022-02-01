using System;

using FluentValidation;

namespace Livescore.Application.Livescore.Discussion.Commands.UnsubscribeFromDiscussion {
    public class UnsubscribeFromDiscussionCommandValidator : AbstractValidator<UnsubscribeFromDiscussionCommand> {
        public UnsubscribeFromDiscussionCommandValidator() {
            RuleFor(c => c.FixtureId).GreaterThan(0);
            RuleFor(c => c.TeamId).GreaterThan(0);
            RuleFor(c => c.DiscussionId).Must(value => Guid.TryParse(value, out Guid _));
        }
    }
}
