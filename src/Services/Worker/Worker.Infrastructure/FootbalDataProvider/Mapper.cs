using System;
using System.Collections.Generic;
using System.Linq;

using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;
using Worker.Infrastructure.FootballDataProvider.Dto;

namespace Worker.Infrastructure.FootballDataProvider {
    public class Mapper {
        public CountryDto Map(
            FetchCountriesResponseDto.CountryDto dto,
            Action<CountryDto> postMap = null
        ) {
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

        public TeamDto Map(
            TeamResponseDto.TeamDto dto,
            Action<TeamDto> postMap = null
        ) {
            if (dto == null) {
                return null;
            }

            var t = new TeamDto {
                Id = dto.Id,
                Name = dto.Name,
                CountryId = dto.CountryId,
                LogoUrl = dto.LogoPath,
                Venue = Map(dto.Venue.Data, postMap: v => v.TeamId = dto.Id),
                Manager = Map(dto.Coach?.Data, postMap: m => m.TeamId = dto.Id)
            };

            postMap?.Invoke(t);

            return t;
        }

        public VenueDto Map(
            TeamResponseDto.TeamDto.VenueDataDto.VenueDto dto,
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
            TeamResponseDto.TeamDto.CoachDataDto.CoachDto dto,
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
