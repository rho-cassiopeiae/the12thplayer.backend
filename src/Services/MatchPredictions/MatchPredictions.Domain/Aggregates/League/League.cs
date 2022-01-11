using System.Collections.Generic;

using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.League {
    public class League : Entity, IAggregateRoot {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public bool? IsCup { get; private set; }
        public string LogoUrl { get; private set; }
        
        private List<Season> _seasons = new();
        public IReadOnlyList<Season> Seasons => _seasons;

        public League(long id, string name, string type, bool? isCup, string logoUrl) {
            Id = id;
            Name = name;
            Type = type;
            IsCup = isCup;
            LogoUrl = logoUrl;
        }

        public void AddSeason(Season season) {
            _seasons.Add(season);
        }
    }
}
