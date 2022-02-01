using FluentValidation;

namespace Livescore.Application.Livescore.VideoReaction.Commands.PostVideoReaction {
    public class PostVideoReactionCommandValidator : AbstractValidator<PostVideoReactionCommand> {
        public PostVideoReactionCommandValidator() {
            RuleFor(c => c.FixtureId).GreaterThan(0);
            RuleFor(c => c.TeamId).GreaterThan(0);
        }
    }
}
