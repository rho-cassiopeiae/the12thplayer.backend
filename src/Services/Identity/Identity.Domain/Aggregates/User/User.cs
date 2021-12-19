using System.Security.Claims;
using System.Collections.Generic;

using Identity.Domain.Base;

namespace Identity.Domain.Aggregates.User {
    public class User : Entity, IAggregateRoot {
        public long Id { get; private set; }
        public string Email { get; private set; }
        public string Username { get; private set; }
        public bool IsConfirmed { get; private set; }

        private readonly List<Claim> _claims = new();
        public IReadOnlyList<Claim> Claims => _claims;

        private readonly List<RefreshToken> _refreshTokens = new();
        public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens;
        
        public User(string email, string username) {
            Email = email;
            Username = username;
        }

        public User(long id, string email, string username, bool isConfirmed) {
            Id = id;
            Email = email;
            Username = username;
            IsConfirmed = isConfirmed;
        }

        public void SetConfirmed(bool confirmed = true) {
            IsConfirmed = confirmed;
        }

        public void AddClaim(string name, string value) {
            _claims.Add(new Claim(name, value));
        }

        public void RemoveClaim(string name) {
            _claims.RemoveAll(claim => claim.Type == name);
        }

        public void AddRefreshToken(RefreshToken token) {
            _refreshTokens.Add(token);
        }

        public void RemoveRefreshToken(RefreshToken token) {
            _refreshTokens.Remove(token);
        }

        public void RemoveAllRefreshTokensForDevice(string deviceId) {
            _refreshTokens.RemoveAll(t => t.DeviceId == deviceId);
        }
    }
}
