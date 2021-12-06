using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Discussion {
    public interface IDiscussionInMemRepository : IInMemRepository<Discussion> {
        void Create(Discussion discussion);
    }
}
