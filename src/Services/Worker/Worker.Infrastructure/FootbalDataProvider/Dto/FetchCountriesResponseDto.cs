using System.Collections.Generic;

namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public class FetchCountriesResponseDto : ResponseDto {
        public class CountryDto {
            public long Id { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
        }

        public IEnumerable<CountryDto> Data { get; set; }
    }
}
