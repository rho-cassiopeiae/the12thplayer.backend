using FluentValidation;

namespace MatchPredictions.Application.Playtime.Commands.SubmitMatchPredictions {
    public class SubmitMatchPredictionsCommandValidator : AbstractValidator<SubmitMatchPredictionsCommand> {
        public SubmitMatchPredictionsCommandValidator() {
            RuleFor(c => c.SeasonId).GreaterThan(0);
            RuleFor(c => c.RoundId).GreaterThan(0);
            RuleForEach(c => c.FixtureIdToScore).Must(kv =>
                long.TryParse(kv.Key, out _) &&
                kv.Value.Length == 2 &&
                short.TryParse(kv.Value, out _)
            );
        }
    }
}
