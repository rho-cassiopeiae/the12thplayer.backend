using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;
using MassTransit;
using MediatR;

using MessageBus.Contracts.Requests.Worker;
using MessageBus.Contracts.Responses.Livescore;
using PlayerDtoMsg = MessageBus.Contracts.Common.Dto.PlayerDto;

using Livescore.Application.Common.Dto;
using Livescore.Application.Livescore.Worker.Queries.GetTeamPlayers;

namespace Livescore.Api.Consumers.Worker {
    public class QueryRequestsConsumer : IConsumer<GetTeamPlayers> {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public QueryRequestsConsumer(ISender mediator, IMapper mapper) {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<GetTeamPlayers> context) {
            var query = new GetTeamPlayersQuery {
                TeamId = context.Message.TeamId
            };

            var result = await _mediator.Send(query);

            await context.RespondAsync(new GetTeamPlayersSuccess {
                CorrelationId = Guid.NewGuid(),
                Players = _mapper.Map<IEnumerable<PlayerDto>, IEnumerable<PlayerDtoMsg>>(
                    result.Data
                )
            });
        }
    }
}
