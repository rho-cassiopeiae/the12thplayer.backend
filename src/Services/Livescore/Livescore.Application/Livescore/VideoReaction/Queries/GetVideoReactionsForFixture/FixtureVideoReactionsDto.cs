using System.Collections.Generic;

namespace Livescore.Application.Livescore.VideoReaction.Queries.GetVideoReactionsForFixture {
    public class FixtureVideoReactionsDto {
        public int Page { get; init; }
        public int TotalPages { get; init; }
        public IEnumerable<VideoReactionWithUserVoteDto> VideoReactions { get; init; }
    }
}
