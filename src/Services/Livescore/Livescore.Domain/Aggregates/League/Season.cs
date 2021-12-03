using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.League {
    public class Season : Entity {
        public long Id { get; private set; }
        public long LeagueId { get; private set; }
        public string Name { get; private set; }
        public bool IsCurrent { get; private set; }
        
        public Season(long id, string name, bool isCurrent) {
            Id = id;
            Name = name;
            IsCurrent = isCurrent;
        }
    }
}
