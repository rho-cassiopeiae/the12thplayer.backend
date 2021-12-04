using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using MassTransit;
using MediatR;
using AutoMapper;

using MessageBus.Contracts.Requests.Worker;
using MessageBus.Contracts.Responses.Livescore;
using CountryDtoMsg = MessageBus.Contracts.Common.Dto.CountryDto;
using TeamDtoMsg = MessageBus.Contracts.Common.Dto.TeamDto;
using FixtureDtoMsg = MessageBus.Contracts.Common.Dto.FixtureDto;
using SeasonDtoMsg = MessageBus.Contracts.Common.Dto.SeasonDto;
using PlayerDtoMsg = MessageBus.Contracts.Common.Dto.PlayerDto;

using Livescore.Application.Common.Dto;
using Livescore.Application.Seed.Commands.AddTeamDetails;
using Livescore.Application.Seed.Commands.AddCountries;
using Livescore.Application.Seed.Commands.AddTeamFinishedFixtures;
using Livescore.Application.Seed.Commands.AddTeamUpcomingFixtures;
using Livescore.Application.Seed.Commands.AddTeamPlayers;

namespace Livescore.Api.Consumers.Worker {
    public class SeedRequestsConsumer :
        IConsumer<AddCountries>,
        IConsumer<AddTeamDetails>,
        IConsumer<AddTeamFinishedFixtures>,
        IConsumer<AddTeamUpcomingFixtures>,
        IConsumer<AddTeamPlayers> {
        private readonly IMapper _mapper;
        private readonly ISender _mediator;

        public SeedRequestsConsumer(IMapper mapper, ISender mediator) {
            _mapper = mapper;
            _mediator = mediator;
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

        public async Task Consume(ConsumeContext<AddTeamDetails> context) {
            var command = new AddTeamDetailsCommand {
                Team = _mapper.Map<TeamDtoMsg, TeamDto>(context.Message.Team)
            };

            await _mediator.Send(command);

            await context.RespondAsync(new AddTeamDetailsSuccess {
                CorrelationId = Guid.NewGuid()
            });
        }

        public async Task Consume(ConsumeContext<AddTeamFinishedFixtures> context) {
            var command = new AddTeamFinishedFixturesCommand {
                TeamId = context.Message.TeamId,
                Fixtures = _mapper.Map<IEnumerable<FixtureDtoMsg>, IEnumerable<FixtureDto>>(
                    context.Message.Fixtures
                ),
                Seasons = _mapper.Map<IEnumerable<SeasonDtoMsg>, IEnumerable<SeasonDto>>(
                    context.Message.Seasons
                ),
                Players = _mapper.Map<IEnumerable<PlayerDtoMsg>, IEnumerable<PlayerDto>>(
                    context.Message.Players
                )
            };

            await _mediator.Send(command);

            await context.RespondAsync(new AddTeamFinishedFixturesSuccess {
                CorrelationId = Guid.NewGuid()
            });
        }

        public async Task Consume(ConsumeContext<AddTeamUpcomingFixtures> context) {
            var command = new AddTeamUpcomingFixturesCommand {
                TeamId = context.Message.TeamId,
                Fixtures = _mapper.Map<IEnumerable<FixtureDtoMsg>, IEnumerable<FixtureDto>>(
                    context.Message.Fixtures
                ),
                Seasons = _mapper.Map<IEnumerable<SeasonDtoMsg>, IEnumerable<SeasonDto>>(
                    context.Message.Seasons
                )
            };

            await _mediator.Send(command);

            await context.RespondAsync(new AddTeamUpcomingFixturesSuccess {
                CorrelationId = Guid.NewGuid()
            });
        }

        public async Task Consume(ConsumeContext<AddTeamPlayers> context) {
            var command = new AddTeamPlayersCommand {
                TeamId = context.Message.TeamId,
                Players = _mapper.Map<IEnumerable<PlayerDtoMsg>, IEnumerable<PlayerDto>>(
                    context.Message.Players
                )
            };

            await _mediator.Send(command);

            await context.RespondAsync(new AddTeamPlayersSuccess {
                CorrelationId = Guid.NewGuid()
            });
        }
    }
}
