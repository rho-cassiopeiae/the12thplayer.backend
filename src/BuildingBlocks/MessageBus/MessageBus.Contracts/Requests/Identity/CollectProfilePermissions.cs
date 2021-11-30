namespace MessageBus.Contracts.Requests.Identity {
    public class CollectProfilePermissions : Message {
        public long UserId { get; set; }
    }
}
