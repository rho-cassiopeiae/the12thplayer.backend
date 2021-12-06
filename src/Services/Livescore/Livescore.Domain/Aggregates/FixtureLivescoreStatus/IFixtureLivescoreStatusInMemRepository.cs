using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.FixtureLivescoreStatus {
    public interface IFixtureLivescoreStatusInMemRepository : IInMemRepository<FixtureLivescoreStatus> {
        void SetOrUpdate(FixtureLivescoreStatus fixtureStatus);
    }
}
