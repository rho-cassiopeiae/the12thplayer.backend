using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using Worker.Infrastructure.FootballDataProvider.Utils;

namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public class SeasonDto {
        public class LeagueDataDto {
            public class LeagueDto {
                public long Id { get; set; }
                public string Name { get; set; }
                public string Type { get; set; }
                public bool? IsCup { get; set; }
                public string LogoPath { get; set; }
            }

            public LeagueDto Data { get; set; }
        }

        public class RoundsDataDto {
            public class RoundDto {
                public class FixturesDataDto {
                    public IEnumerable<FixtureDto> Data { get; set; }
                }

                public long Id { get; set; }
                public int Name { get; set; }
                [JsonConverter(typeof(RoundDateJsonConverter))]
                public DateTime? Start { get; set; }
                [JsonConverter(typeof(RoundDateJsonConverter))]
                public DateTime? End { get; set; }

                public FixturesDataDto Fixtures { get; set; }
            }

            public IEnumerable<RoundDto> Data { get; set; }
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? CurrentRoundId { get; set; }
        public LeagueDataDto League { get; set; }
        public RoundsDataDto Rounds { get; set; }
    }
}
