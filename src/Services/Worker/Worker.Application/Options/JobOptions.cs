using System.Collections.Generic;

namespace Worker.Application.Options {
    public class JobOptions {
        public string Name { get; set; }
        public string Type { get; set; }
        public string CronSchedule { get; set; }
        public bool Durable { get; set; }
        public IDictionary<string, string> DataMap { get; set; }
        public string ExecuteAfter { get; set; }
    }
}
