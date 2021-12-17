﻿using System;

namespace Feed.Domain.Aggregates.Author {
    [Flags]
    public enum JobManagementPermissions : short {
        ExecuteOneOffJobs = 1 << 0,
        SchedulePeriodicJobs = 1 << 1
    }
}
