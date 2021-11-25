namespace Identity.Infrastructure.Account.Persistence.Models {
    public class IntegrationEvent {
        public int Id { get; set; }
        public IntegrationEventType Type { get; set; }
        public string Content { get; set; }
        public IntegrationEventStatus Status { get; set; }
    }
}
