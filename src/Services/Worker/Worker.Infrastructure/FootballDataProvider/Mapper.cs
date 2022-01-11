using System;
using System.Collections.Generic;
using System.Linq;

using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;
using Worker.Infrastructure.FootballDataProvider.Dto;
using FixtureDtoApp = Worker.Application.Jobs.OneOff.FootballDataCollection.Dto.FixtureDto;
using FixtureDtoResponse = Worker.Infrastructure.FootballDataProvider.Dto.FixtureDto;
using SeasonDtoApp = Worker.Application.Jobs.OneOff.FootballDataCollection.Dto.SeasonDto;
using SeasonDtoResponse = Worker.Infrastructure.FootballDataProvider.Dto.SeasonDto;

namespace Worker.Infrastructure.FootballDataProvider {
    public class Mapper {
        public CountryDto Map(GetCountriesResponseDto.CountryDto dto, Action<CountryDto> postMap = null) {
            if (dto == null) {
                return null;
            }

            var c = new CountryDto {
                Id = dto.Id,
                Name = dto.Name,
                FlagUrl = dto.ImagePath
            };

            postMap?.Invoke(c);

            return c;
        }

        public TeamDto Map(GetTeamDetailsResponseDto.TeamDto dto, Action<TeamDto> postMap = null) {
            if (dto == null) {
                return null;
            }

            var t = new TeamDto {
                Id = dto.Id,
                Name = dto.Name,
                CountryId = dto.CountryId,
                LogoUrl = dto.LogoPath,
                Venue = Map(dto.Venue.Data),
                Manager = Map(dto.Coach?.Data)
            };

            postMap?.Invoke(t);

            return t;
        }

        public VenueDto Map(
            GetTeamDetailsResponseDto.TeamDto.VenueDataDto.VenueDto dto,
            Action<VenueDto> postMap = null
        ) {
            if (dto == null) {
                return null;
            }

            var v = new VenueDto {
                Id = dto.Id,
                Name = dto.Name,
                City = dto.City,
                Capacity = dto.Capacity,
                ImageUrl = dto.ImagePath
            };

            postMap?.Invoke(v);

            return v;
        }

        public ManagerDto Map(
            GetTeamDetailsResponseDto.TeamDto.CoachDataDto.CoachDto dto,
            Action<ManagerDto> postMap = null
        ) {
            if (dto == null) {
                return null;
            }

            var m = new ManagerDto {
                Id = dto.CoachId,
                FirstName = dto.Firstname,
                LastName = dto.Lastname,
                BirthDate = dto.Birthdate,
                CountryId = dto.CountryId,
                ImageUrl = dto.ImagePath
            };

            postMap?.Invoke(m);

            return m;
        }

        public SeasonDtoApp Map(SeasonDtoResponse dto, Action<SeasonDtoApp> postMap = null) {
            if (dto == null) {
                return null;
            }

            var s = new SeasonDtoApp {
                Id = dto.Id,
                Name = dto.Name,
                CurrentRoundId = dto.CurrentRoundId,
                League = Map(dto.League.Data),
                Rounds = Map<RoundDto>(dto.Rounds?.Data)
            };

            postMap?.Invoke(s);

            return s;
        }

        public LeagueDto Map(SeasonDtoResponse.LeagueDataDto.LeagueDto dto, Action<LeagueDto> postMap = null) {
            if (dto == null) {
                return null;
            }

            var l = new LeagueDto {
                Id = dto.Id,
                Name = dto.Name,
                Type = dto.Type,
                IsCup = dto.IsCup,
                LogoUrl = dto.LogoPath
            };

            postMap?.Invoke(l);

            return l;
        }

        public RoundDto Map(SeasonDtoResponse.RoundsDataDto.RoundDto dto, Action<RoundDto> postMap = null) {
            if (dto == null) {
                return null;
            }

            var r = new RoundDto {
                Id = dto.Id,
                Name = dto.Name,
                StartDate = dto.Start,
                EndDate = dto.End,
                Fixtures = Map<FixtureForMatchPredictionDto>(dto.Fixtures.Data, postMap: f => f.RoundId = dto.Id)
            };

            postMap?.Invoke(r);

            return r;
        }

        public FixtureForMatchPredictionDto Map(FixtureDtoResponse dto, Action<FixtureForMatchPredictionDto> postMap = null) {
            if (dto == null) {
                return null;
            }

            var f = new FixtureForMatchPredictionDto {
                Id = dto.Id,
                StartTime = dto.MatchStatus.StartingAt?.DateTime?.DateTime ?? DateTime.MinValue,
                Status = dto.MatchStatus.Status,
                GameTime = Map(dto.MatchStatus),
                Score = Map(dto.Scores ?? new FixtureDtoResponse.ScoreDto()),
                HomeTeam = Map(dto.LocalTeam.Data),
                GuestTeam = Map(dto.VisitorTeam.Data),
            };

            postMap?.Invoke(f);

            return f;
        }

        public PlayerDto Map(GetPlayerResponseDto.PlayerDto dto, Action<PlayerDto> postMap = null) {
            if (dto == null) {
                return null;
            }

            var p = new PlayerDto {
                Id = dto.PlayerId,
                FirstName = dto.Firstname,
                LastName = dto.Lastname,
                DisplayName = dto.DisplayName,
                BirthDate = dto.Birthdate,
                CountryId = dto.CountryId,
                Position = dto.Position?.Data.Name,
                ImageUrl = dto.ImagePath
            };

            postMap?.Invoke(p);

            return p;
        }

        public FixtureDtoApp Map(FixtureDtoResponse dto, long teamId, Action<FixtureDtoApp> postMap = null) {
            if (dto == null) {
                return null;
            }

            bool homeStatus = teamId == dto.LocalTeam.Data.Id;
            var team = homeStatus ? dto.LocalTeam.Data : dto.VisitorTeam.Data;
            var opponentTeam = homeStatus ? dto.VisitorTeam.Data : dto.LocalTeam.Data;

            var startTime = dto.MatchStatus.StartingAt?.DateTime?.DateTime ?? DateTime.MinValue;

            var f = new FixtureDtoApp {
                Id = dto.Id,
                SeasonId = dto.SeasonId,
                HomeStatus = homeStatus,
                StartTime = startTime,
                Status = dto.MatchStatus.Status,
                GameTime = Map(dto.MatchStatus),
                Score = Map(dto.Scores ?? new FixtureDtoResponse.ScoreDto()),
                RefereeName = dto.Referee?.Data.CommonName,
                Colors = new[] {
                    new TeamColorDto {
                        TeamId = teamId,
                        Color = homeStatus ? dto.Colors?.Localteam?.Color : dto.Colors?.Visitorteam?.Color
                    },
                    new TeamColorDto {
                        TeamId = opponentTeam.Id,
                        Color = homeStatus ? dto.Colors?.Visitorteam?.Color : dto.Colors?.Localteam?.Color
                    }
                },
                Lineups = new[] {
                    new TeamLineupDto {
                        TeamId = teamId,
                        Formation = homeStatus ? dto.Formations?.LocalteamFormation : dto.Formations?.VisitorteamFormation,
                        Manager = homeStatus ? Map(dto.LocalCoach?.Data) : Map(dto.VisitorCoach?.Data),
                        StartingXI = dto.Lineups?.Data
                            .Where(player => player.TeamId == teamId && player.PlayerId != null)
                            .Select(player => Map(player, postMap: p => p.FixtureStartTime = startTime))
                            .ToList(),
                        Subs = dto.Bench?.Data
                            .Where(player => player.TeamId == teamId && player.PlayerId != null)
                            .Select(player => Map(player, postMap: p => p.FixtureStartTime = startTime))
                            .ToList()
                    },
                    new TeamLineupDto {
                        TeamId = opponentTeam.Id,
                        Formation = homeStatus ? dto.Formations?.VisitorteamFormation : dto.Formations?.LocalteamFormation,
                        Manager = homeStatus ? Map(dto.VisitorCoach?.Data) : Map(dto.LocalCoach?.Data),
                        StartingXI = dto.Lineups?.Data
                            .Where(player => player.TeamId == opponentTeam.Id && player.PlayerId != null)
                            .Select(player => Map(player, postMap: p => p.FixtureStartTime = startTime))
                            .ToList(),
                        Subs = dto.Bench?.Data
                            .Where(player => player.TeamId == opponentTeam.Id && player.PlayerId != null)
                            .Select(player => Map(player, postMap: p => p.FixtureStartTime = startTime))
                            .ToList()
                    }
                },
                Events = new[] {
                    new TeamMatchEventsDto {
                        TeamId = teamId,
                        Events = dto.Events?.Data
                            .Where(@event => long.Parse(@event.TeamId) == teamId)
                            .Select(@event => Map(@event))
                            .ToList()
                    },
                    new TeamMatchEventsDto {
                        TeamId = opponentTeam.Id,
                        Events = dto.Events?.Data
                            .Where(@event => long.Parse(@event.TeamId) == opponentTeam.Id)
                            .Select(@event => Map(@event))
                            .ToList()
                    }
                },
                Stats = new[] {
                    new TeamStatsDto {
                        TeamId = teamId,
                        Stats = Map(dto.Stats?.Data.FirstOrDefault(stats => stats.TeamId == teamId)),
                    },
                    new TeamStatsDto {
                        TeamId = opponentTeam.Id,
                        Stats = Map(dto.Stats?.Data.FirstOrDefault(stats => stats.TeamId == opponentTeam.Id)),
                    }
                },
                OpponentTeam = Map(opponentTeam),
                Venue = Map(
                    dto.Venue?.Data,
                    postMap: v => v.TeamId =
                        dto.Venue.Data.Id == team.VenueId ?
                            team.Id :
                            dto.Venue.Data.Id == opponentTeam.VenueId ? opponentTeam.Id : null
                )
            };

            postMap?.Invoke(f);

            return f;
        }

        public GameTimeDto Map(FixtureDtoResponse.MatchStatusDto dto, Action<GameTimeDto> postMap = null) {
            if (dto == null) {
                return null;
            }

            var g = new GameTimeDto {
                Minute = dto.Minute,
                ExtraTimeMinute = dto.ExtraMinute,
                AddedTimeMinute = dto.InjuryTime
            };

            postMap?.Invoke(g);

            return g;
        }

        public ScoreDto Map(FixtureDtoResponse.ScoreDto dto, Action<ScoreDto> postMap = null) {
            if (dto == null) {
                return null;
            }

            var s = new ScoreDto {
                LocalTeam = dto.LocalteamScore.GetValueOrDefault(),
                VisitorTeam = dto.VisitorteamScore.GetValueOrDefault(),
                HT = string.IsNullOrEmpty(dto.HtScore) ? null : dto.HtScore,
                FT = string.IsNullOrEmpty(dto.FtScore) ? null : dto.FtScore,
                ET = string.IsNullOrEmpty(dto.EtScore) ? null : dto.EtScore,
                PS = string.IsNullOrEmpty(dto.PsScore) ? null : dto.PsScore
            };

            postMap?.Invoke(s);

            return s;
        }

        public TeamLineupDto.ManagerDto Map(
            FixtureDtoResponse.CoachDataDto.CoachDto dto, Action<TeamLineupDto.ManagerDto> postMap = null
        ) {
            if (dto == null) {
                return null;
            }

            var m = new TeamLineupDto.ManagerDto {
                Id = dto.CoachId,
                Name = dto.CommonName,
                ImageUrl = dto.ImagePath
            };

            postMap?.Invoke(m);

            return m;
        }

        public TeamLineupDto.PlayerDto Map(
            FixtureDtoResponse.PlayersDataDto.PlayerDto dto, Action<TeamLineupDto.PlayerDto> postMap = null
        ) {
            if (dto == null) {
                return null;
            }

            var p = new TeamLineupDto.PlayerDto {
                Id = dto.PlayerId.Value,
                Number = dto.Number.GetValueOrDefault(),
                IsCaptain = dto.Captain.GetValueOrDefault(),
                Position = dto.Position,
                FormationPosition = dto.FormationPosition,
                Rating = dto.Stats?.Rating != null ? float.Parse(dto.Stats.Rating) : null
            };

            postMap?.Invoke(p);

            return p;
        }

        public TeamMatchEventsDto.MatchEventDto Map(
            FixtureDtoResponse.MatchEventsDataDto.MatchEventDto dto, Action<TeamMatchEventsDto.MatchEventDto> postMap = null
        ) {
            if (dto == null) {
                return null;
            }

            var e = new TeamMatchEventsDto.MatchEventDto {
                Minute = dto.Minute,
                AddedTimeMinute = dto.ExtraMinute,
                Type = dto.Type,
                PlayerId = dto.PlayerId,
                RelatedPlayerId = dto.RelatedPlayerId
            };

            postMap?.Invoke(e);

            return e;
        }

        public TeamStatsDto.StatsDto Map(
            FixtureDtoResponse.StatsDataDto.TeamStatsDto dto, Action<TeamStatsDto.StatsDto> postMap = null
        ) {
            if (dto == null) {
                return null;
            }

            var s = new TeamStatsDto.StatsDto {
                Shots = Map(dto.Shots),
                Passes = Map(dto.Passes),
                Fouls = dto.Fouls,
                Corners = dto.Corners,
                Offsides = dto.Offsides,
                BallPossession = (short?) dto.Possessiontime,
                YellowCards = dto.Yellowcards,
                RedCards = dto.Redcards,
                Tackles = dto.Tackles
            };

            postMap?.Invoke(s);

            return s;
        }

        public TeamStatsDto.StatsDto.ShotStatsDto Map(
            FixtureDtoResponse.StatsDataDto.TeamStatsDto.ShotStatsDto dto, Action<TeamStatsDto.StatsDto.ShotStatsDto> postMap = null
        ) {
            if (dto == null) {
                return null;
            }

            var s = new TeamStatsDto.StatsDto.ShotStatsDto {
                Total = dto.Total,
                OnTarget = dto.Ongoal,
                OffTarget = dto.Offgoal,
                Blocked = dto.Blocked,
                InsideBox = dto.Insidebox,
                OutsideBox = dto.Outsidebox
            };

            postMap?.Invoke(s);

            return s;
        }

        public TeamStatsDto.StatsDto.PassStatsDto Map(
            FixtureDtoResponse.StatsDataDto.TeamStatsDto.PassStatsDto dto, Action<TeamStatsDto.StatsDto.PassStatsDto> postMap = null
        ) {
            if (dto == null) {
                return null;
            }

            var p = new TeamStatsDto.StatsDto.PassStatsDto {
                Total = dto.Total,
                Accurate = dto.Accurate
            };

            postMap?.Invoke(p);

            return p;
        }

        public TeamDto Map(FixtureDtoResponse.TeamDataDto.TeamDto dto, Action<TeamDto> postMap = null) {
            if (dto == null) {
                return null;
            }

            var t = new TeamDto {
                Id = dto.Id,
                Name = dto.Name,
                CountryId = dto.CountryId,
                LogoUrl = dto.LogoPath
            };

            postMap?.Invoke(t);

            return t;
        }

        public VenueDto Map(FixtureDtoResponse.VenueDataDto.VenueDto dto, Action<VenueDto> postMap = null) {
            if (dto == null) {
                return null;
            }

            var v = new VenueDto {
                Id = dto.Id,
                Name = dto.Name,
                City = dto.City,
                Capacity = dto.Capacity,
                ImageUrl = dto.ImagePath
            };

            postMap?.Invoke(v);

            return v;
        }

        public IEnumerable<TDest> Map<TDest>(
            IEnumerable<dynamic> dtos,
            dynamic arg = null,
            Action<dynamic> postMap = null
        ) {
            if (arg == null) {
                return dtos?.Select(dto => (TDest) Map(dto, postMap: postMap));
            } else {
                return dtos?.Select(dto => (TDest) Map(dto, arg, postMap: postMap));
            }
        }
    }
}
