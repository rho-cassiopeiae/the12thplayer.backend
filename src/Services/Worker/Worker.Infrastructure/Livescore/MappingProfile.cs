using AutoMapper;

using CountryDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.CountryDto;
using TeamDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.TeamDto;
using VenueDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.VenueDto;
using ManagerDtoMsg = MessageBus.Contracts.Requests.Worker.Dto.ManagerDto;

using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Infrastructure.Livescore {
    public class MappingProfile : Profile {
        public MappingProfile() {
            CreateMap<CountryDto, CountryDtoMsg>();
            CreateMap<TeamDto, TeamDtoMsg>();
            CreateMap<VenueDto, VenueDtoMsg>();
            CreateMap<ManagerDto, ManagerDtoMsg>();
        }
    }
}
