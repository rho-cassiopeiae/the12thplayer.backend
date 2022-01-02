using System.Collections.Generic;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection.Dto {
    public class FixturePlayerRatingsDto {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public IEnumerable<PlayerRatingDto> PlayerRatings { get; set; }
    }
}
