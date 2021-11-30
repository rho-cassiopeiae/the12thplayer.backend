using System;

namespace Profile.Domain.Aggregates.Profile {
    [Flags]
    public enum JobManagementPermissions {
        ExecuteOneOffJobs = 1 << 0,
        SchedulePeriodicJobs = 1 << 1
    }
}
