using System;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection.Dto {
    public class PlayerDto {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public long? CountryId { get; set; }
        public short Number { get; set; }
        public string Position { get; set; }
        public string ImageUrl { get; set; }
        public DateTime LastLineupAt { get; set; }
    }
}
