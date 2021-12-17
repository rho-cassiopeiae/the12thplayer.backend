using System;

namespace Feed.Domain.Aggregates.Author {
    [Flags]
    public enum AdminPanelPermissions : short {
        LogIn = 1 << 0
    }
}
