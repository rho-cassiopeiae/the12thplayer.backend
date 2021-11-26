using System;

namespace Identity.Domain.Aggregates.User {
    public class RefreshToken {
        public string Value { get; private set; }
        public bool IsActive { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }
        
        public bool IsValid =>
            IsActive && DateTimeOffset.UtcNow.CompareTo(ExpiresAt) < 0;
        
        public RefreshToken(string value, bool isActive, DateTimeOffset expiresAt) {
            Value = value;
            IsActive = isActive;
            ExpiresAt = expiresAt;
        }
    }
}
