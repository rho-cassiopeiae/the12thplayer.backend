using System;

namespace Profile.Domain.Aggregates.Profile {
    [Flags]
    public enum AdminPanelPermissions {
        LogIn = 1 << 0
    }
}
