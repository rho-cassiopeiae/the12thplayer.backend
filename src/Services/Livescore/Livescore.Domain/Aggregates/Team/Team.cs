using System;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Team {
    public class Team : Entity, IAggregateRoot, IEquatable<Team> {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public long CountryId { get; private set; }
        public string LogoUrl { get; private set; }
        public bool HasThe12thPlayerCommunity { get; private set; }
        
        public Team(
            long id, string name, long countryId,
            string logoUrl, bool hasThe12thPlayerCommunity
        ) {
            Id = id;
            Name = name;
            CountryId = countryId;
            LogoUrl = logoUrl;
            HasThe12thPlayerCommunity = hasThe12thPlayerCommunity;
        }

        public void ChangeName(string name) {
            Name = name;
        }

        public void ChangeLogo(string logoUrl) {
            LogoUrl = logoUrl;
        }

        public void SetHasThe12thPlayerCommunity(bool hasThe12thPlayerCommunity) {
            HasThe12thPlayerCommunity = hasThe12thPlayerCommunity;
        }

        public bool Equals(Team other) => other != null && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}
