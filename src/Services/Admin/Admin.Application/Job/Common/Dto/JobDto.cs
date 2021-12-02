using System.Collections.Generic;

namespace Admin.Application.Job.Common.Dto {
    public class JobDto {
        public string Name { get; set; }
        public string Type { get; set; }
        public string CronSchedule { get; set; }
        public IDictionary<string, object> DataMap { get; set; }
        public string ExecuteAfter { get; set; }
    }
}
