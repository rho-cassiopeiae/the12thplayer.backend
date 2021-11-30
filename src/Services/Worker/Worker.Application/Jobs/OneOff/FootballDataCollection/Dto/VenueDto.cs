using System;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection.Dto {
    public class VenueDto : IEquatable<VenueDto> {
        public long Id { get; set; }
        public long? TeamId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int? Capacity { get; set; }
        public string ImageUrl { get; set; }

        public bool Equals(VenueDto other) => other != null && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}
