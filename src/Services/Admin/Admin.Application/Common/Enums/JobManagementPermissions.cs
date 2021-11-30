using System;

namespace Admin.Application.Common.Enums {
    [Flags]
    public enum JobManagementPermissions {
        ExecuteOneOffJobs = 1 << 0,
        SchedulePeriodicJobs = 1 << 1
    }
}
