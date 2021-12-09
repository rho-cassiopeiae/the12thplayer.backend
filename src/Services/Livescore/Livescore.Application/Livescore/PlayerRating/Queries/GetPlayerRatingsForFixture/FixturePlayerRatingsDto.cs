using System.Collections.Generic;

namespace Livescore.Application.Livescore.PlayerRating.Queries.GetPlayerRatingsForFixture {
    public class FixturePlayerRatingsDto {
        public bool RatingsFinalized { get; init; }
        public IEnumerable<PlayerRatingWithUserVoteDto> PlayerRatings { get; init; }
    }
}
