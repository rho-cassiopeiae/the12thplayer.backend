using System;

namespace MatchPredictions.Application.Common.Dto {
    public class TeamDto : IEquatable<TeamDto> {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CountryId { get; set; }
        public string LogoUrl { get; set; }

        public bool Equals(TeamDto other) => other != null && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}
