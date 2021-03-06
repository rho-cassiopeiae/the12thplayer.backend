using System;
using System.Text.Json;

using Identity.Domain.Base;

namespace Identity.Application.Common.Integration {
    public class IntegrationEvent : IAggregateRoot, IDisposable {
        public Guid Id { get; set; }
        public IntegrationEventType Type { get; set; }
        public JsonDocument Payload { get; set; }
        public IntegrationEventStatus Status { get; set; }

        public void Dispose() => Payload?.Dispose();

        // @@TODO: Finalizer ?
    }
}
