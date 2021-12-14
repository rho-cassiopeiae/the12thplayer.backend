using System;

namespace Admin.Application.Common.Enums {
    [Flags]
    public enum AdminPanelPermissions {
        LogIn = 1 << 0
    }
}
