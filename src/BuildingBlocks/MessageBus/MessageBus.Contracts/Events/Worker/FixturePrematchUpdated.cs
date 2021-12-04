using MessageBus.Contracts.Common.Dto;

namespace MessageBus.Contracts.Events.Worker {
    public class FixturePrematchUpdated : Message {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public FixtureDto Fixture { get; set; }
    }
}
