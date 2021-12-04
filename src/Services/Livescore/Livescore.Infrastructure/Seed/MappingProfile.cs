using AutoMapper;

using CountryDtoMsg = MessageBus.Contracts.Common.Dto.CountryDto;
using TeamDtoMsg = MessageBus.Contracts.Common.Dto.TeamDto;
using VenueDtoMsg = MessageBus.Contracts.Common.Dto.VenueDto;
using ManagerDtoMsg = MessageBus.Contracts.Common.Dto.ManagerDto;
using FixtureDtoMsg = MessageBus.Contracts.Common.Dto.FixtureDto;
using GameTimeDtoMsg = MessageBus.Contracts.Common.Dto.GameTimeDto;
using ScoreDtoMsg = MessageBus.Contracts.Common.Dto.ScoreDto;
using TeamColorDtoMsg = MessageBus.Contracts.Common.Dto.TeamColorDto;
using TeamLineupDtoMsg = MessageBus.Contracts.Common.Dto.TeamLineupDto;
using TeamMatchEventsDtoMsg = MessageBus.Contracts.Common.Dto.TeamMatchEventsDto;
using TeamStatsDtoMsg = MessageBus.Contracts.Common.Dto.TeamStatsDto;
using SeasonDtoMsg = MessageBus.Contracts.Common.Dto.SeasonDto;
using LeagueDtoMsg = MessageBus.Contracts.Common.Dto.LeagueDto;
using PlayerDtoMsg = MessageBus.Contracts.Common.Dto.PlayerDto;

using Livescore.Application.Common.Dto;

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

            CreateMap<PlayerDtoMsg, PlayerDto>()
                .ReverseMap();
        }
    }
}
