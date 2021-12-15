using System.Collections.Generic;
using System.Linq;

using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.Author {
    public class Author : Entity, IAggregateRoot {
        public long UserId { get; private set; }
        public string Email { get; private set; }
        public string Username { get; private set; }

        private List<AuthorPermission> _permissions = new();
        public IReadOnlyList<AuthorPermission> Permissions => _permissions;

        public Author(long userId, string email, string username) {
            UserId = userId;
            Email = email;
            Username = username;
        }

        public void AddPermission(PermissionScope scope, int flags) {
            // @@TODO: Check validity.
            var valid = false;
            switch (scope) {
                case PermissionScope.AdminPanel:
                    valid = true;
                    break;
                case PermissionScope.UserManagement:
                    valid = true;
                    break;
                case PermissionScope.JobManagement:
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
                _permissions.Add(new AuthorPermission(scope, flags));
            }
        }
    }
}
