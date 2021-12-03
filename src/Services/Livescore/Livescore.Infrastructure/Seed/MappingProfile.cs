using AutoMapper;

using CountryDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.CountryDto;
using TeamDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.TeamDto;
using VenueDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.VenueDto;
using ManagerDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.ManagerDto;
using FixtureDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.FixtureDto;
using GameTimeDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.GameTimeDto;
using ScoreDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.ScoreDto;
using TeamColorDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.TeamColorDto;
using TeamLineupDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.TeamLineupDto;
using TeamMatchEventsDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.TeamMatchEventsDto;
using TeamStatsDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.TeamStatsDto;
using SeasonDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.SeasonDto;
using LeagueDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.LeagueDto;
using PlayerDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.PlayerDto;

using Livescore.Application.Seed.Common.Dto;

namespace Livescore.Infrastructure.Seed {
    public class MappingProfile : Profile {
        public MappingProfile() {
            CreateMap<CountryDtoMsg, CountryDto>();
            CreateMap<TeamDtoMsg, TeamDto>();
            CreateMap<VenueDtoMsg, VenueDto>();
            CreateMap<ManagerDtoMsg, ManagerDto>();

            CreateMap<FixtureDtoMsg, FixtureDto>();
            CreateMap<GameTimeDtoMsg, GameTimeDto>();
            CreateMap<ScoreDtoMsg, ScoreDto>();
            CreateMap<TeamColorDtoMsg, TeamColorDto>();
            CreateMap<TeamLineupDtoMsg, TeamLineupDto>();
            CreateMap<TeamLineupDtoMsg.PlayerDto, TeamLineupDto.PlayerDto>();
            CreateMap<TeamLineupDtoMsg.ManagerDto, TeamLineupDto.ManagerDto>();
            CreateMap<TeamMatchEventsDtoMsg, TeamMatchEventsDto>();
            CreateMap<TeamMatchEventsDtoMsg.MatchEventDto, TeamMatchEventsDto.MatchEventDto>();
            CreateMap<TeamStatsDtoMsg, TeamStatsDto>();
            CreateMap<TeamStatsDtoMsg.StatsDto, TeamStatsDto.StatsDto>();
            CreateMap<TeamStatsDtoMsg.StatsDto.ShotStatsDto, TeamStatsDto.StatsDto.ShotStatsDto>();
            CreateMap<TeamStatsDtoMsg.StatsDto.PassStatsDto, TeamStatsDto.StatsDto.PassStatsDto>();

            CreateMap<SeasonDtoMsg, SeasonDto>();
            CreateMap<LeagueDtoMsg, LeagueDto>();

            CreateMap<PlayerDtoMsg, PlayerDto>();
        }
    }
}
