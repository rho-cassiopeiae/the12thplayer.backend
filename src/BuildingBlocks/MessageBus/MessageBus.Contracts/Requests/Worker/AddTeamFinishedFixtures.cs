using System.Collections.Generic;

using MessageBus.Contracts.Requests.Worker.Dto;

namespace MessageBus.Contracts.Requests.Worker {
    public class AddTeamFinishedFixtures : Message {
        public long TeamId { get; set; }
        public IEnumerable<FixtureDto> Fixtures { get; set; }
        public IEnumerable<SeasonDto> Seasons { get; set; }
        public IEnumerable<PlayerDto> Players { get; set; }
    }
}
