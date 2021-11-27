using System;

namespace Profile.Domain.Aggregates.Profile {
    [Flags]
    public enum UserManagementPermissions {
        ListUsers = 1 << 0,
        GrantPermission = 1 << 1,
        RevokePermission = 1 << 2
    }
}
