namespace MessageBus.Contracts.Events.Worker {
    public class FixtureFinished : Message {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
    }
}
