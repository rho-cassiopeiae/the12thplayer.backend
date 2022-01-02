namespace Worker.Application.Jobs.OneOff.FootballDataCollection.Dto {
    public class PlayerRatingDto {
        public string ParticipantKey { get; set; }
        public float? Rating { get; set; }
        public int TotalRating { get; set; }
        public int TotalVoters { get; set; }
    }
}
