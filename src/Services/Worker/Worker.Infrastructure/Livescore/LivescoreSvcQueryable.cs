using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;
using MassTransit;

using MessageBus.Contracts.Requests.Worker;
using MessageBus.Contracts.Responses.Livescore;
using PlayerDtoMsg = MessageBus.Contracts.Common.Dto.PlayerDto;

using Worker.Application.Common.Interfaces;
using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Infrastructure.Livescore {
    public class LivescoreSvcQueryable : ILivescoreSvcQueryable {
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly Uri _destinationAddress;

        public LivescoreSvcQueryable(IBus bus, IMapper mapper) {
            _bus = bus;
            _mapper = mapper;
            _destinationAddress = new Uri("queue:livescore-query-requests"); // @@TODO: Config.
        }

        public async Task<IEnumerable<PlayerDto>> GetTeamPlayers(long teamId) {
            var client = _bus.CreateRequestClient<GetTeamPlayers>(_destinationAddress);

            var response = await client.GetResponse<GetTeamPlayersSuccess>(
                new GetTeamPlayers {
                    CorrelationId = Guid.NewGuid(),
                    TeamId = teamId
                }
            );

            return _mapper.Map<IEnumerable<PlayerDtoMsg>, IEnumerable<PlayerDto>>(
                response.Message.Players
            );
        }
    }
}
