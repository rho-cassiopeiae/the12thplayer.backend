using FluentValidation;

namespace Livescore.Application.Livescore.Fixture.Queries.GetFixturesForTeamInBetween {
    public class GetFixturesForTeamInBetweenQueryValidator : AbstractValidator<GetFixturesForTeamInBetweenQuery> {
        public GetFixturesForTeamInBetweenQueryValidator() {
            RuleFor(q => q.TeamId).GreaterThan(0);
            RuleFor(q => new { q.StartTime, q.EndTime })
                .Must(interval => interval.StartTime > 0 && interval.EndTime > interval.StartTime)
                .WithName("Interval")
                .WithMessage("Invalid [start time; end time] interval");
        }
    }
}
