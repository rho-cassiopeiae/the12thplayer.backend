namespace Livescore.Application.Seed.Common.Dto {
    public class TeamDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CountryId { get; set; }
        public string LogoUrl { get; set; }
        public VenueDto Venue { get; set; }
        public ManagerDto Manager { get; set; }
    }
}
