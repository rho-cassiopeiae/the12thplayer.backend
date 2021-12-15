using System;

namespace Feed.Domain.Aggregates.Author {
    [Flags]
    public enum AdminPanelPermissions {
        LogIn = 1 << 0
    }
}
