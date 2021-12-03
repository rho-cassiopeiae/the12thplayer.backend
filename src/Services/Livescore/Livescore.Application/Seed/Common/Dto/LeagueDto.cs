using System;

namespace Livescore.Application.Seed.Common.Dto {
    public class LeagueDto : IEquatable<LeagueDto> {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool? IsCup { get; set; }
        public string LogoUrl { get; set; }

        public bool Equals(LeagueDto other) => other != null && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}
