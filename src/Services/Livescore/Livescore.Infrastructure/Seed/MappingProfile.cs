using AutoMapper;

using CountryDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.CountryDto;
using TeamDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.TeamDto;
using VenueDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.VenueDto;
using ManagerDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.ManagerDto;

using Livescore.Application.Seed.Common.Dto;

namespace Livescore.Infrastructure.Seed {
    public class MappingProfile : Profile {
        public MappingProfile() {
            CreateMap<CountryDtoMsg, CountryDto>();
            CreateMap<TeamDtoMsg, TeamDto>();
            CreateMap<VenueDtoMsg, VenueDto>();
            CreateMap<ManagerDtoMsg, ManagerDto>();
        }
    }
}
