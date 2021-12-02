using System;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Country {
    public class Country : Entity, IAggregateRoot, IEquatable<Country> {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public string FlagUrl { get; private set; }
        
        public Country(long id, string name, string flagUrl) {
            Id = id;
            Name = name;
            FlagUrl = flagUrl;
        }
        
        public bool Equals(Country other) => other != null && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}
