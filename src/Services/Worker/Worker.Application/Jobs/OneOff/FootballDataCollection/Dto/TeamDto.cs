﻿using System;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection.Dto {
    public class TeamDto : IEquatable<TeamDto> {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CountryId { get; set; }
        public string LogoUrl { get; set; }
        public bool HasCommunity { get; set; }
        public VenueDto Venue { get; set; }
        public ManagerDto Manager { get; set; }

        public bool Equals(TeamDto other) => other != null && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}
