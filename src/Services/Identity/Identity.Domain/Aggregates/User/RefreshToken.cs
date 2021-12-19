using System;

using Identity.Domain.Base;

namespace Identity.Domain.Aggregates.User {
    public class RefreshToken : Entity {
        public string DeviceId { get; private set; }
        public string Value { get; private set; }
        public bool IsActive { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }
        
        public bool IsValid => IsActive && DateTimeOffset.UtcNow.CompareTo(ExpiresAt) < 0;
        
        public RefreshToken(
            string deviceId, string value, bool isActive, DateTimeOffset expiresAt
        ) {
            DeviceId = deviceId;
            Value = value;
            IsActive = isActive;
            ExpiresAt = expiresAt;
        }
    }
}
