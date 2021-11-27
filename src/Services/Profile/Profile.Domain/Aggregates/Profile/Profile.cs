using System.Collections.Generic;
using System.Linq;

using Profile.Domain.Base;

namespace Profile.Domain.Aggregates.Profile {
    public class Profile : IAggregateRoot {
        public long UserId { get; private set; }
        public string Email { get; private set; }
        public string Username { get; private set; }
        public int Reputation { get; private set; }

        private List<ProfilePermission> _permissions = new();
        public IReadOnlyList<ProfilePermission> Permissions => _permissions;

        public Profile(
            long userId, string email, string username, int reputation = 0
        ) {
            UserId = userId;
            Email = email;
            Username = username;
            Reputation = reputation;
        }

        public void AddPermission(PermissionScope scope, int flags) {
            // @@TODO: Check validity.
            var valid = false;
            switch (scope) {
                case PermissionScope.UserManagement:
                    valid = true;
                    break;
                case PermissionScope.Article:
                    valid = true;
                    break;
            }

            if (!valid) {

            }

            var permission = _permissions.FirstOrDefault(p => p.Scope == scope);
            if (permission != null) {
                permission.AddFlags(flags);
            } else {
                _permissions.Add(new ProfilePermission(scope, flags));
            }
        }
    }
}
