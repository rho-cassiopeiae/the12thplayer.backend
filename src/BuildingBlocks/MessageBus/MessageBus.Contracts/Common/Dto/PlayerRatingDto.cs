namespace MessageBus.Contracts.Common.Dto {
    public class PlayerRatingDto {
        public string ParticipantKey { get; set; }
        public int TotalRating { get; set; }
        public int TotalVoters { get; set; }
    }
}
