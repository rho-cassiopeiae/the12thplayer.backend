using System;

using FluentValidation;

namespace Livescore.Application.Livescore.Discussion.Commands.PostDiscussionEntry {
    public class PostDiscussionEntryCommandValidator : AbstractValidator<PostDiscussionEntryCommand> {
        public PostDiscussionEntryCommandValidator() {
            RuleFor(c => c.FixtureId).GreaterThan(0);
            RuleFor(c => c.TeamId).GreaterThan(0);
            RuleFor(c => c.DiscussionId).Must(value => Guid.TryParse(value, out Guid _));
            RuleFor(c => c.Body).NotNull().Length(min: 1, max: 300); // @@TODO: Config.
        }
    }
}
