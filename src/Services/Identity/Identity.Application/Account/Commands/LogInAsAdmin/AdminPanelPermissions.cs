using System;

namespace Identity.Application.Account.Commands.LogInAsAdmin {
    [Flags]
    public enum AdminPanelPermissions {
        LogIn = 1 << 0
    }
}
