using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoMapper;
using MassTransit;
using MediatR;

using MessageBus.Contracts.Requests.Worker;
using MessageBus.Contracts.Responses.MatchPredictions;
using CountryDtoMsg = MessageBus.Contracts.Common.Dto.CountryDto;
using SeasonDtoMsg = MessageBus.Contracts.Common.Dto.SeasonDto;

using MatchPredictions.Application.Common.Dto;
using MatchPredictions.Application.Seed.Commands.AddSeasonWithRoundsAndFixtures;
using MatchPredictions.Application.Seed.Commands.AddCountries;

namespace MatchPredictions.Api.Consumers.Worker {
    public class SeedRequestsConsumer :
        IConsumer<AddCountries>,
        IConsumer<AddSeasonWithRoundsAndFixtures> {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public SeedRequestsConsumer(ISender mediator, IMapper mapper) {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AddCountries> context) {
            var command = new AddCountriesCommand {
                Countries = _mapper.Map<IEnumerable<CountryDtoMsg>, IEnumerable<CountryDto>>(
                    context.Message.Countries
                )
            };

            await _mediator.Send(command);

            await context.RespondAsync(new AddCountriesSuccess {
                CorrelationId = Guid.NewGuid()
            });
        }

        public async Task Consume(ConsumeContext<AddSeasonWithRoundsAndFixtures> context) {
            var command = new AddSeasonWithRoundsAndFixturesCommand {
                Season = _mapper.Map<SeasonDtoMsg, SeasonDto>(context.Message.Season)
            };

            await _mediator.Send(command);

            await context.RespondAsync(new AddSeasonWithRoundsAndFixturesSuccess {
                CorrelationId = Guid.NewGuid()
            });
        }
    }
}
