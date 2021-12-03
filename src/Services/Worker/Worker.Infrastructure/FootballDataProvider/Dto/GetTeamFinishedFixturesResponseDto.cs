using System.Collections.Generic;

namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public class GetTeamFinishedFixturesResponseDto : ResponseDto {
        public IEnumerable<FixtureDto> Data { get; set; }
    }
}
