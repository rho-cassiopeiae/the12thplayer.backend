using System;

namespace Feed.Domain.Aggregates.Author {
    [Flags]
    public enum UserManagementPermissions : short {
        ListUsers = 1 << 0,
        GrantPermission = 1 << 1,
        RevokePermission = 1 << 2
    }
}
