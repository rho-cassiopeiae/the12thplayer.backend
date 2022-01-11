using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoMapper;
using MassTransit;

using MessageBus.Contracts.Requests.Worker;
using MessageBus.Contracts.Responses.MatchPredictions;
using CountryDtoMsg = MessageBus.Contracts.Common.Dto.CountryDto;
using SeasonDtoMsg = MessageBus.Contracts.Common.Dto.SeasonDto;

using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;
using Worker.Application.Common.Interfaces;

namespace Worker.Infrastructure.MatchPredictions {
    public class MatchPredictionsSeeder : IMatchPredictionsSeeder {
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly Uri _destinationAddress;

        public MatchPredictionsSeeder(IBus bus, IMapper mapper) {
            _bus = bus;
            _mapper = mapper;
            _destinationAddress = new Uri("queue:match-predictions-seed-requests"); // @@TODO: Config.
        }

        public async Task AddCountries(IEnumerable<CountryDto> countries) {
            var client = _bus.CreateRequestClient<AddCountries>(_destinationAddress);

            await client.GetResponse<AddCountriesSuccess>(new AddCountries {
                CorrelationId = Guid.NewGuid(),
                Countries = _mapper.Map<IEnumerable<CountryDto>, IEnumerable<CountryDtoMsg>>(countries)
            });
        }

        public async Task AddSeasonWithRoundsAndFixtures(SeasonDto season) {
            var client = _bus.CreateRequestClient<AddSeasonWithRoundsAndFixtures>(_destinationAddress);

            await client.GetResponse<AddSeasonWithRoundsAndFixturesSuccess>(new AddSeasonWithRoundsAndFixtures {
                CorrelationId = Guid.NewGuid(),
                Season = _mapper.Map<SeasonDto, SeasonDtoMsg>(season)
            });
        }
    }
}
