namespace MessageBus.Contracts.Requests.Admin {
    public class LogInAsAdmin : Message {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
