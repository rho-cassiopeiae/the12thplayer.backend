namespace MessageBus.Contracts.Requests.Worker {
    public class AddFileFoldersForFixture : Message {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
    }
}
