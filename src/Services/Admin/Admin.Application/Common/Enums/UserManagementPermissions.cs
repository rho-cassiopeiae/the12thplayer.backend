using System;

namespace Admin.Application.Common.Enums {
    [Flags]
    public enum UserManagementPermissions {
        ListUsers = 1 << 0,
        GrantPermission = 1 << 1,
        RevokePermission = 1 << 2
    }
}
