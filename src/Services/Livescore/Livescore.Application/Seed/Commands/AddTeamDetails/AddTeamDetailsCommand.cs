using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.Team;
using Livescore.Domain.Aggregates.Venue;
using Livescore.Domain.Aggregates.Manager;
using Livescore.Application.Common.Dto;
using TeamDm = Livescore.Domain.Aggregates.Team.Team;

namespace Livescore.Application.Seed.Commands.AddTeamDetails {
    public class AddTeamDetailsCommand : IRequest<VoidResult> {
        public TeamDto Team { get; init; }
    }

    public class AddTeamDetailsCommandHandler : IRequestHandler<
        AddTeamDetailsCommand, VoidResult
    > {
        private readonly ITeamRepository _teamRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly IManagerRepository _managerRepository;

        public AddTeamDetailsCommandHandler(
            ITeamRepository teamRepository,
            IVenueRepository venueRepository,
            IManagerRepository managerRepository
        ) {
            _teamRepository = teamRepository;
            _venueRepository = venueRepository;
            _managerRepository = managerRepository;
        }

        public async Task<VoidResult> Handle(
            AddTeamDetailsCommand command, CancellationToken cancellationToken
        ) {
            var teamDto = command.Team;

            var team = await _teamRepository.FindById(teamDto.Id);
            if (team == null) {
                team = new TeamDm(
                    id: teamDto.Id,
                    name: teamDto.Name,
                    countryId: teamDto.CountryId,
                    logoUrl: teamDto.LogoUrl,
                    hasThe12thPlayerCommunity: true
                );

                _teamRepository.Create(team);
            } else {
                if (team.Name != teamDto.Name) {
                    team.ChangeName(teamDto.Name);
                }
                if (team.LogoUrl != teamDto.LogoUrl) {
                    team.ChangeLogo(teamDto.LogoUrl);
                }
                if (!team.HasThe12thPlayerCommunity) {
                    team.SetHasThe12thPlayerCommunity(true);
                }
            }

            var venueDto = teamDto.Venue;

            var venue = await _venueRepository.FindById(venueDto.Id);
            if (venue == null) {
                venue = new Venue(
                    id: venueDto.Id,
                    teamId: team.Id,
                    name: venueDto.Name,
                    city: venueDto.City,
                    capacity: venueDto.Capacity,
                    imageUrl: venueDto.ImageUrl
                );

                _venueRepository.Create(venue);
            } else {
                if (venue.Name != venueDto.Name) {
                    venue.ChangeName(venueDto.Name);
                }
                if (venue.Capacity != venueDto.Capacity) {
                    venue.ChangeCapacity(venueDto.Capacity);
                }
                if (venue.ImageUrl != venueDto.ImageUrl) {
                    venue.ChangeImage(venueDto.ImageUrl);
                }
            }

            var managerDto = teamDto.Manager;

            if (managerDto != null) {
                var manager = await _managerRepository.FindById(managerDto.Id);
                if (manager == null) {
                    manager = new Manager(
                        id: managerDto.Id,
                        teamId: team.Id,
                        firstName: managerDto.FirstName,
                        lastName: managerDto.LastName,
                        birthDate: managerDto.BirthDate != null ?
                            new DateTimeOffset(managerDto.BirthDate.Value).ToUnixTimeMilliseconds() :
                            null,
                        countryId: managerDto.CountryId,
                        imageUrl: managerDto.ImageUrl
                    );

                    _managerRepository.Create(manager);
                } else {
                    if (manager.ImageUrl != managerDto.ImageUrl) {
                        manager.ChangeImage(managerDto.ImageUrl);
                    }
                }
            }

            await _teamRepository.SaveChanges(cancellationToken);

            return VoidResult.Instance;
        }
    }
}
