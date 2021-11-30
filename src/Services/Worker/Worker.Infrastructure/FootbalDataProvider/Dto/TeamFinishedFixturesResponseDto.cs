using System.Collections.Generic;

namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public class TeamFinishedFixturesResponseDto : ResponseDto {
        public IEnumerable<FixtureDto> Data { get; set; }
    }
}
