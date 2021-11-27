namespace MessageBus.Contracts.Events.Identity {
    public class UserAccountCreated : Message {
        public string Email { get; set; }
        public string Username { get; set; }
        public string ConfirmationCode { get; set; }
    }
}
