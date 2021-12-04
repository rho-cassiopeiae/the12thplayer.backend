using System.Collections.Generic;

using MessageBus.Contracts.Common.Dto;

namespace MessageBus.Contracts.Responses.Livescore {
    public class GetTeamPlayersSuccess : Message {
        public IEnumerable<PlayerDto> Players { get; set; }
    }
}
