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

using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Infrastructure.Livescore {
    public class MappingProfile : Profile {
        public MappingProfile() {
            CreateMap<CountryDto, CountryDtoMsg>();
            CreateMap<TeamDto, TeamDtoMsg>();
            CreateMap<VenueDto, VenueDtoMsg>();
            CreateMap<ManagerDto, ManagerDtoMsg>();

            CreateMap<FixtureDto, FixtureDtoMsg>();
            CreateMap<GameTimeDto, GameTimeDtoMsg>();
            CreateMap<ScoreDto, ScoreDtoMsg>();
            CreateMap<TeamColorDto, TeamColorDtoMsg>();
            CreateMap<TeamLineupDto, TeamLineupDtoMsg>();
            CreateMap<TeamLineupDto.PlayerDto, TeamLineupDtoMsg.PlayerDto>();
            CreateMap<TeamLineupDto.ManagerDto, TeamLineupDtoMsg.ManagerDto>();
            CreateMap<TeamMatchEventsDto, TeamMatchEventsDtoMsg>();
            CreateMap<TeamMatchEventsDto.MatchEventDto, TeamMatchEventsDtoMsg.MatchEventDto>();
            CreateMap<TeamStatsDto, TeamStatsDtoMsg>();
            CreateMap<TeamStatsDto.StatsDto, TeamStatsDtoMsg.StatsDto>();
            CreateMap<TeamStatsDto.StatsDto.ShotStatsDto, TeamStatsDtoMsg.StatsDto.ShotStatsDto>();
            CreateMap<TeamStatsDto.StatsDto.PassStatsDto, TeamStatsDtoMsg.StatsDto.PassStatsDto>();

            CreateMap<SeasonDto, SeasonDtoMsg>();
            CreateMap<LeagueDto, LeagueDtoMsg>();

            CreateMap<PlayerDto, PlayerDtoMsg>();
        }
    }
}
