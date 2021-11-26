using System;
using System.Text.Json;

namespace Identity.Infrastructure.Account.Persistence.Models {
    public class IntegrationEvent : IDisposable {
        public int Id { get; set; }
        public IntegrationEventType Type { get; set; }
        public JsonDocument Payload { get; set; }
        public IntegrationEventStatus Status { get; set; }

        public void Dispose() => Payload?.Dispose();
    }
}
