using System.Collections.Generic;

namespace MessageBus.Contracts.Commands.Admin {
    public class ExecuteOneOffJobs : Message {
        public class Job {
            public string Name { get; set; }
            public string Type { get; set; }
            public IDictionary<string, string> DataMap { get; set; }
            public string ExecuteAfter { get; set; }
        }

        public IEnumerable<Job> Jobs { get; set; }
    }
}
