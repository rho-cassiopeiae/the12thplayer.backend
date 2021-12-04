namespace MessageBus.Contracts.Requests.Worker {
    public class GetTeamPlayers : Message {
        public long TeamId { get; set; }
    }
}
