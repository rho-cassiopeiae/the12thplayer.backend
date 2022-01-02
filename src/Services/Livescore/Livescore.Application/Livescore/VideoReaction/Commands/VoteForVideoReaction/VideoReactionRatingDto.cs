using System.Text.Json.Serialization;

namespace Livescore.Application.Livescore.VideoReaction.Commands.VoteForVideoReaction {
    public class VideoReactionRatingDto {
        public int Rating { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public short? UserVote { get; init; }
    }
}
