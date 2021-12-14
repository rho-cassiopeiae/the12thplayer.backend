using System;

namespace Profile.Domain.Aggregates.Profile {
    [Flags]
    public enum ArticlePermissions {
        Publish = 1 << 0,
        Review = 1 << 1,
        Edit = 1 << 2,
        Delete = 1 << 3
    }
}
