using System.Collections.Generic;

namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public class FixtureLivescoreResponseDto : ResponseDto {
        public IEnumerable<FixtureDto> Data { get; set; }
    }
}
