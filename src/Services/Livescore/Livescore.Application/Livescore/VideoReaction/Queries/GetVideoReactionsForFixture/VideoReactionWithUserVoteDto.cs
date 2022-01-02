using System.Text.Json.Serialization;

namespace Livescore.Application.Livescore.VideoReaction.Queries.GetVideoReactionsForFixture {
    public class VideoReactionWithUserVoteDto {
        public long AuthorId { get; init; }
        public string Title { get; init; }
        public string AuthorUsername { get; init; }
        public int Rating { get; init; }
        public string VideoId { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public short? UserVote { get; set; }
    }
}
