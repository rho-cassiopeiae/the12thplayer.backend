using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.League {
    public class Season : Entity {
        public long Id { get; private set; }
        public long LeagueId { get; private set; }
        public string Name { get; private set; }
        
        public Season(long id, string name) {
            Id = id;
            Name = name;
        }
    }
}
