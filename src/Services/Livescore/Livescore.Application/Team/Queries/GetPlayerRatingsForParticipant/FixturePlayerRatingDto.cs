namespace Livescore.Application.Team.Queries.GetPlayerRatingsForParticipant {
    public class FixturePlayerRatingDto {
        public string OpponentTeamName { get; set; }
        public string OpponentTeamLogoUrl { get; set; }
        public long FixtureStartTime { get; set; }
        public bool FixtureHomeStatus { get; set; }
        public short HomeTeamScore { get; set; }
        public short GuestTeamScore { get; set; }
        public int TotalRating { get; set; }
        public int TotalVoters { get; set; }
    }
}
