namespace MessageBus.Contracts.Requests.Identity {
    public class GetProfilePermissions : Message {
        public long UserId { get; set; }
    }
}
