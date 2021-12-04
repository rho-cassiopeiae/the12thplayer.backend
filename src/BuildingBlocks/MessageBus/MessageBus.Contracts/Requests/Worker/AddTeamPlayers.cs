using System.Collections.Generic;

using MessageBus.Contracts.Common.Dto;

namespace MessageBus.Contracts.Requests.Worker {
    public class AddTeamPlayers : Message {
        public long TeamId { get; set; }
        public IEnumerable<PlayerDto> Players { get; set; }
    }
}
