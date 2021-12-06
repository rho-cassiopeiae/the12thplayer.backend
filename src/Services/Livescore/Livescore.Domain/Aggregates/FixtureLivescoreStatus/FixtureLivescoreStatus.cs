using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.FixtureLivescoreStatus {
    public class FixtureLivescoreStatus : Entity, IAggregateRoot {
        public long FixtureId { get; private set; }
        public long TeamId { get; private set; }
        public bool? Active { get; private set; }
        public bool? Ongoing { get; private set; }
        
        public FixtureLivescoreStatus(long fixtureId, long teamId, bool? active, bool? ongoing) {
            FixtureId = fixtureId;
            TeamId = teamId;
            Active = active;
            Ongoing = ongoing;
        }
    }
}
