using FluentValidation;

namespace Livescore.Application.Livescore.VideoReaction.Commands.VoteForVideoReaction {
    public class VoteForVideoReactionCommandValidator : AbstractValidator<VoteForVideoReactionCommand> {
        public VoteForVideoReactionCommandValidator() {
            RuleFor(c => c.FixtureId).GreaterThan(0);
            RuleFor(c => c.TeamId).GreaterThan(0);
            RuleFor(c => c.AuthorId).GreaterThan(0);
            RuleFor(c => c.UserVote).Must(value => value is null or 1 or -1);
        }
    }
}
