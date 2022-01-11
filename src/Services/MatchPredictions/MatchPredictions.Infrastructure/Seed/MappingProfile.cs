using AutoMapper;

using CountryDtoMsg = MessageBus.Contracts.Common.Dto.CountryDto;
using TeamDtoMsg = MessageBus.Contracts.Common.Dto.TeamDto;
using GameTimeDtoMsg = MessageBus.Contracts.Common.Dto.GameTimeDto;
using ScoreDtoMsg = MessageBus.Contracts.Common.Dto.ScoreDto;
using SeasonDtoMsg = MessageBus.Contracts.Common.Dto.SeasonDto;
using LeagueDtoMsg = MessageBus.Contracts.Common.Dto.LeagueDto;
using RoundDtoMsg = MessageBus.Contracts.Common.Dto.RoundDto;
using FixtureForMatchPredictionDtoMsg = MessageBus.Contracts.Common.Dto.FixtureForMatchPredictionDto;

using MatchPredictions.Application.Common.Dto;

namespace MatchPredictions.Infrastructure.Seed {
    public class MappingProfile : Profile {
        public MappingProfile() {
            CreateMap<CountryDtoMsg, CountryDto>();

            CreateMap<SeasonDtoMsg, SeasonDto>();
            CreateMap<LeagueDtoMsg, LeagueDto>();
            CreateMap<RoundDtoMsg, RoundDto>();
            CreateMap<FixtureForMatchPredictionDtoMsg, FixtureForMatchPredictionDto>();
            CreateMap<GameTimeDtoMsg, GameTimeDto>();
            CreateMap<ScoreDtoMsg, ScoreDto>();
            CreateMap<TeamDtoMsg, TeamDto>();
        }
    }
}
