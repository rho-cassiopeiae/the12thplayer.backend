namespace MessageBus.Contracts.Common.Dto {
    public class VenueDto {
        public long Id { get; set; }
        public long? TeamId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int? Capacity { get; set; }
        public string ImageUrl { get; set; }
    }
}
