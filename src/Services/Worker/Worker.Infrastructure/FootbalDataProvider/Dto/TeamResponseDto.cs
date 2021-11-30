using System;
using System.Text.Json.Serialization;

using Worker.Infrastructure.FootballDataProvider.Utils;

namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public class TeamResponseDto : ResponseDto {
        public class TeamDto {
            public class VenueDataDto {
                public class VenueDto {
                    public long Id { get; set; }
                    public string Name { get; set; }
                    public string City { get; set; }
                    public int? Capacity { get; set; }
                    public string ImagePath { get; set; }
                }

                public VenueDto Data { get; set; }
            }

            public class CoachDataDto {
                public class CoachDto {
                    public long CoachId { get; set; }
                    public string Firstname { get; set; }
                    public string Lastname { get; set; }
                    public long? CountryId { get; set; }
                    [JsonConverter(typeof(BirthDateJsonConverter))]
                    public DateTime? Birthdate { get; set; }
                    public string ImagePath { get; set; }
                }

                public CoachDto Data { get; set; }
            }

            public long Id { get; set; }
            public string Name { get; set; }
            public long CountryId { get; set; }
            public string LogoPath { get; set; }
            public VenueDataDto Venue { get; set; }
            public CoachDataDto Coach { get; set; }
        }

        public TeamDto Data { get; set; }
    }
}
