namespace MessageBus.Contracts.Events.Worker {
    public class FixtureActivated : Message {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public string VimeoProjectId { get; set; }
    }
}
