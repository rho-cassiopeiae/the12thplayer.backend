using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Discussion {
    public interface IDiscussionInMemRepository : IInMemRepository<Discussion> {
        Task<IEnumerable<Discussion>> FindAllFor(long fixtureId, long teamId);

        void WatchStillActive(Discussion discussion);

        void Create(Discussion discussion);

        void Delete(long fixtureId, long teamId, List<Guid> discussionIds);

        void PostEntries(Discussion discussion);
    }
}
