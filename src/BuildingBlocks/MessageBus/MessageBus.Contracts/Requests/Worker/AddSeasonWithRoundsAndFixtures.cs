using MessageBus.Contracts.Common.Dto;

namespace MessageBus.Contracts.Requests.Worker {
    public class AddSeasonWithRoundsAndFixtures : Message {
        public SeasonDto Season { get; set; }
    }
}
