using System;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection.Dto {
    public class CountryDto : IEquatable<CountryDto> {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FlagUrl { get; set; }

        public bool Equals(CountryDto other) => other != null && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}
