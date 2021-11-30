using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using Worker.Infrastructure.FootballDataProvider.Utils;

namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public class FixtureDto {
        public class TeamDataDto {
            public class TeamDto {
                public long Id { get; set; }
                public string Name { get; set; }
                public long CountryId { get; set; }
                public string LogoPath { get; set; }
                public long VenueId { get; set; }
            }

            public TeamDto Data { get; set; }
        }

        public class MatchStatusDto {
            public class TimeDto {
                [JsonConverter(typeof(StartTimeJsonConverter))]
                public DateTimeOffset? DateTime { get; set; }
            }

            public string Status { get; set; }
            public TimeDto StartingAt { get; set; }
            public short? Minute { get; set; }
            public short? ExtraMinute { get; set; }
            public short? InjuryTime { get; set; }
        }

        public class ScoreDto {
            public short? LocalteamScore { get; set; }
            public short? VisitorteamScore { get; set; }
            public string HtScore { get; set; }
            public string FtScore { get; set; }
            public string EtScore { get; set; }
            public string PsScore { get; set; }
        }

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

        public class RefereeDataDto {
            public class RefereeDto {
                public string CommonName { get; set; }
            }

            public RefereeDto Data { get; set; }
        }

        public class ColorsDto {
            public class ColorDto {
                public string Color { get; set; }
            }

            public ColorDto Localteam { get; set; }
            public ColorDto Visitorteam { get; set; }
        }

        public class FormationsDto {
            public string LocalteamFormation { get; set; }
            public string VisitorteamFormation { get; set; }
        }

        public class CoachDataDto {
            public class CoachDto {
                public long CoachId { get; set; }
                public string CommonName { get; set; }
                public string ImagePath { get; set; }
            }

            public CoachDto Data { get; set; }
        }

        public class PlayersDataDto {
            public class PlayerDto {
                public class PlayerStatsDto {
                    public string Rating { get; set; }
                }

                public long TeamId { get; set; }
                public long PlayerId { get; set; }
                public string PlayerName { get; set; }
                public short Number { get; set; }
                public string Position { get; set; }
                public short? FormationPosition { get; set; }
                public bool? Captain { get; set; }
                public PlayerStatsDto Stats { get; set; }
            }

            public IEnumerable<PlayerDto> Data { get; set; }
        }

        public class MatchEventsDataDto {
            public class MatchEventDto {
                public string TeamId { get; set; }
                public short Minute { get; set; }
                public short? ExtraMinute { get; set; }
                public string Type { get; set; }
                public long? PlayerId { get; set; }
                public long? RelatedPlayerId { get; set; }
            }

            public IEnumerable<MatchEventDto> Data { get; set; }
        }

        public class StatsDataDto {
            public class TeamStatsDto {
                public class ShotStatsDto {
                    public short? Total { get; set; }
                    public short? Ongoal { get; set; }
                    public short? Offgoal { get; set; }
                    public short? Blocked { get; set; }
                    public short? Insidebox { get; set; }
                    public short? Outsidebox { get; set; }
                }

                public class PassStatsDto {
                    public short? Total { get; set; }
                    public short? Accurate { get; set; }
                }

                public long TeamId { get; set; }
                public ShotStatsDto Shots { get; set; }
                public PassStatsDto Passes { get; set; }
                public short? Fouls { get; set; }
                public short? Corners { get; set; }
                public short? Offsides { get; set; }
                public float? Possessiontime { get; set; }
                public short? Yellowcards { get; set; }
                public short? Redcards { get; set; }
                public short? Tackles { get; set; }
            }

            public IEnumerable<TeamStatsDto> Data { get; set; }
        }

        public long Id { get; set; }

        [JsonPropertyName("localTeam")]
        public TeamDataDto LocalTeam { get; set; }

        [JsonPropertyName("visitorTeam")]
        public TeamDataDto VisitorTeam { get; set; }

        public long? SeasonId { get; set; }

        [JsonPropertyName("time")]
        public MatchStatusDto MatchStatus { get; set; }

        public ScoreDto Scores { get; set; }

        public VenueDataDto Venue { get; set; }

        public RefereeDataDto Referee { get; set; }

        public ColorsDto Colors { get; set; }

        public FormationsDto Formations { get; set; }

        [JsonPropertyName("localCoach")]
        public CoachDataDto LocalCoach { get; set; }

        [JsonPropertyName("visitorCoach")]
        public CoachDataDto VisitorCoach { get; set; }

        [JsonPropertyName("lineup")]
        public PlayersDataDto Lineups { get; set; }

        public PlayersDataDto Bench { get; set; }

        public MatchEventsDataDto Events { get; set; }

        public StatsDataDto Stats { get; set; }

        public bool? Deleted { get; set; }
    }
}
