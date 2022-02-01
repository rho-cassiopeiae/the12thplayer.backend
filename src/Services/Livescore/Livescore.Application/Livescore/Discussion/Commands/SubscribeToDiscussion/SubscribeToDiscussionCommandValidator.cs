using System;

using FluentValidation;

namespace Livescore.Application.Livescore.Discussion.Commands.SubscribeToDiscussion {
    public class SubscribeToDiscussionCommandValidator : AbstractValidator<SubscribeToDiscussionCommand> {
        public SubscribeToDiscussionCommandValidator() {
            RuleFor(c => c.FixtureId).GreaterThan(0);
            RuleFor(c => c.TeamId).GreaterThan(0);
            RuleFor(c => c.DiscussionId).Must(value => Guid.TryParse(value, out Guid _));
        }
    }
}
