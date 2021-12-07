using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.FixtureLivescoreStatus {
    public interface IFixtureLivescoreStatusInMemRepository : IInMemRepository<FixtureLivescoreStatus> {
        void SetOrUpdate(FixtureLivescoreStatus fixtureStatus);
        Task<bool> FindOutIfActive(long fixtureId, long teamId);
        void WatchStillActive(long fixtureId, long teamId);
    }
}
