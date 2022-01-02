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
using FixturePlayerRatingsDtoMsg = MessageBus.Contracts.Common.Dto.FixturePlayerRatingsDto;
using PlayerRatingDtoMsg = MessageBus.Contracts.Common.Dto.PlayerRatingDto;

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

            CreateMap<PlayerDto, PlayerDtoMsg>()
                .ReverseMap();

            CreateMap<FixturePlayerRatingsDto, FixturePlayerRatingsDtoMsg>();
            CreateMap<PlayerRatingDto, PlayerRatingDtoMsg>();
        }
    }
}
