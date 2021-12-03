using System.Collections.Generic;

using MessageBus.Contracts.Requests.Worker.Dto;

namespace MessageBus.Contracts.Requests.Worker {
    public class AddTeamUpcomingFixtures : Message {
        public long TeamId { get; set; }
        public IEnumerable<FixtureDto> Fixtures { get; set; }
        public IEnumerable<SeasonDto> Seasons { get; set; }
    }
}
