using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.Team {
    public class Team : Entity, IAggregateRoot {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public long CountryId { get; private set; }
        public string LogoUrl { get; private set; }
        public TeamActiveSeasons ActiveSeasons { get; private set; }

        public Team(long id, string name, long countryId, string logoUrl) {
            Id = id;
            Name = name;
            CountryId = countryId;
            LogoUrl = logoUrl;
        }

        public void SetActiveSeasons(TeamActiveSeasons activeSeasons) {
            ActiveSeasons = activeSeasons;
        }
    }
}
