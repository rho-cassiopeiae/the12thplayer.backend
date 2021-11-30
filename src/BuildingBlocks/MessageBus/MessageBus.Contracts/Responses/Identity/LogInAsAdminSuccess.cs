namespace MessageBus.Contracts.Responses.Identity {
    public class LogInAsAdminSuccess : Message {
        public string AccessToken { get; set; }
    }
}
