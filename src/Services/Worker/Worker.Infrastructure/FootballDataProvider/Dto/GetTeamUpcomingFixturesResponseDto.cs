using System.Collections.Generic;

namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public class GetTeamUpcomingFixturesResponseDto : ResponseDto {
        public class TeamDto {
            public class UpcomingFixturesDataDto {
                public IEnumerable<FixtureDto> Data { get; set; }
            }

            public UpcomingFixturesDataDto Upcoming { get; set; }
        }

        public TeamDto Data { get; set; }
    }
}
