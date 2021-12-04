﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;
using MassTransit;

using MessageBus.Contracts.Requests.Worker;
using MessageBus.Contracts.Responses.Livescore;
using CountryDtoMsg = MessageBus.Contracts.Common.Dto.CountryDto;
using TeamDtoMsg = MessageBus.Contracts.Common.Dto.TeamDto;
using FixtureDtoMsg = MessageBus.Contracts.Common.Dto.FixtureDto;
using SeasonDtoMsg = MessageBus.Contracts.Common.Dto.SeasonDto;
using PlayerDtoMsg = MessageBus.Contracts.Common.Dto.PlayerDto;

using Worker.Application.Common.Interfaces;
using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Infrastructure.Livescore {
    public class LivescoreSeeder : ILivescoreSeeder {
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly Uri _destinationAddress;

        public LivescoreSeeder(IBus bus, IMapper mapper) {
            _bus = bus;
            _mapper = mapper;
            _destinationAddress = new Uri("queue:livescore-seed-requests"); // @@TODO: Config.
        }

        public async Task AddCountries(IEnumerable<CountryDto> countries) {
            var client = _bus.CreateRequestClient<AddCountries>(_destinationAddress);

            await client.GetResponse<AddCountriesSuccess>(new AddCountries {
                CorrelationId = Guid.NewGuid(),
                Countries = _mapper.Map<IEnumerable<CountryDto>, IEnumerable<CountryDtoMsg>>(countries)
            });
        }

        public async Task AddTeamDetails(TeamDto team) {
            var client = _bus.CreateRequestClient<AddTeamDetails>(_destinationAddress);

            await client.GetResponse<AddTeamDetailsSuccess>(new AddTeamDetails {
                CorrelationId = Guid.NewGuid(),
                Team = _mapper.Map<TeamDto, TeamDtoMsg>(team)
            });
        }

        public async Task AddTeamFinishedFixtures(
            long teamId,
            IEnumerable<FixtureDto> fixtures,
            IEnumerable<SeasonDto> seasons,
            IEnumerable<PlayerDto> players
        ) {
            var client = _bus.CreateRequestClient<AddTeamFinishedFixtures>(_destinationAddress);

            await client.GetResponse<AddTeamFinishedFixturesSuccess>(new AddTeamFinishedFixtures {
                CorrelationId = Guid.NewGuid(),
                TeamId = teamId,
                Fixtures = _mapper.Map<IEnumerable<FixtureDto>, IEnumerable<FixtureDtoMsg>>(fixtures),
                Seasons = _mapper.Map<IEnumerable<SeasonDto>, IEnumerable<SeasonDtoMsg>>(seasons),
                Players = _mapper.Map<IEnumerable<PlayerDto>, IEnumerable<PlayerDtoMsg>>(players)
            });
        }

        public async Task AddTeamUpcomingFixtures(
            long teamId,
            IEnumerable<FixtureDto> fixtures,
            IEnumerable<SeasonDto> seasons
        ) {
            var client = _bus.CreateRequestClient<AddTeamUpcomingFixtures>(_destinationAddress);

            await client.GetResponse<AddTeamUpcomingFixturesSuccess>(new AddTeamUpcomingFixtures {
                CorrelationId = Guid.NewGuid(),
                TeamId = teamId,
                Fixtures = _mapper.Map<IEnumerable<FixtureDto>, IEnumerable<FixtureDtoMsg>>(fixtures),
                Seasons = _mapper.Map<IEnumerable<SeasonDto>, IEnumerable<SeasonDtoMsg>>(seasons)
            });
        }

        public async Task AddTeamPlayers(long teamId, IEnumerable<PlayerDto> players) {
            var client = _bus.CreateRequestClient<AddTeamPlayers>(_destinationAddress);

            await client.GetResponse<AddTeamPlayersSuccess>(new AddTeamPlayers {
                CorrelationId = Guid.NewGuid(),
                TeamId = teamId,
                Players = _mapper.Map<IEnumerable<PlayerDto>, IEnumerable<PlayerDtoMsg>>(players)
            });
        }
    }
}
