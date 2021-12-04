using System.Collections.Generic;

namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public class GetFixtureLivescoreResponseDto : ResponseDto {
        public IEnumerable<FixtureDto> Data { get; set; }
    }
}
