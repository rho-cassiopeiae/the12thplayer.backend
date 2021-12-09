namespace MessageBus.Contracts.Events.Worker {
    public class FixtureDeactivated : Message {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
    }
}
