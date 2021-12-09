using System.Text.Json.Serialization;

namespace Livescore.Application.Livescore.PlayerRating.Queries.GetPlayerRatingsForFixture {
    public class PlayerRatingWithUserVoteDto {
        public string ParticipantKey { get; init; }
        public int TotalRating { get; init; }
        public int TotalVoters { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? UserRating { get; set; }
    }
}
