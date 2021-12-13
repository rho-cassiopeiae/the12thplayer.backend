using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Aggregates.Discussion;

namespace Livescore.Application.Common.Interfaces {
    public interface IDiscussionInMemQueryable {
        Task<IEnumerable<Discussion>> GetAllFor(long fixtureId, long teamId);
        Task<IEnumerable<DiscussionEntry>> GetEntriesFor(
            long fixtureId, long teamId, Guid discussionId, string startFromEntryId
        );
    }
}
