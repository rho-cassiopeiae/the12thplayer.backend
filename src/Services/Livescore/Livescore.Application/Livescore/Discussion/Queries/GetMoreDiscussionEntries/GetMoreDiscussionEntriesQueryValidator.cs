using System;

using FluentValidation;

namespace Livescore.Application.Livescore.Discussion.Queries.GetMoreDiscussionEntries {
    public class GetMoreDiscussionEntriesQueryValidator : AbstractValidator<GetMoreDiscussionEntriesQuery> {
        public GetMoreDiscussionEntriesQueryValidator() {
            RuleFor(q => q.FixtureId).GreaterThan(0);
            RuleFor(q => q.TeamId).GreaterThan(0);
            RuleFor(q => q.DiscussionId).Must(value => Guid.TryParse(value, out Guid _));
            RuleFor(q => q.LastReceivedEntryId).Must(value => {
                var valueSplit = value?.Split('-');
                return valueSplit == null ||
                    valueSplit.Length == 2 &&
                    long.TryParse(valueSplit[0], out long _) &&
                    int.TryParse(valueSplit[1], out int _);
            });
        }
    }
}
