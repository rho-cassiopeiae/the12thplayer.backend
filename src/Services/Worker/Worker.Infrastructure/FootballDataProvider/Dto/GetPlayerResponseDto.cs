using System;
using System.Text.Json.Serialization;

using Worker.Infrastructure.FootballDataProvider.Utils;

namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public class GetPlayerResponseDto : ResponseDto {
        public class PlayerDto {
            public class PositionDataDto {
                public class PositionDto {
                    public string Name { get; set; }
                }

                public PositionDto Data { get; set; }
            }

            public long PlayerId { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string DisplayName { get; set; }
            [JsonConverter(typeof(BirthDateJsonConverter))]
            public DateTime? Birthdate { get; set; }
            public long? CountryId { get; set; }
            public string ImagePath { get; set; }
            public PositionDataDto Position { get; set; }
        }

        public PlayerDto Data { get; set; }
    }
}
