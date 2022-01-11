using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.Country {
    public class Country : Entity, IAggregateRoot {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public string FlagUrl { get; private set; }
        
        public Country(long id, string name, string flagUrl) {
            Id = id;
            Name = name;
            FlagUrl = flagUrl;
        }
    }
}
