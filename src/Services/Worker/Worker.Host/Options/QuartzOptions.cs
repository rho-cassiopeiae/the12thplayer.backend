using System.Collections.Generic;

using Worker.Application.Options;

namespace Worker.Host.Options {
    public class QuartzOptions {
        public int MaxConcurrency { get; set; }
        public IEnumerable<JobOptions> Jobs { get; set; }
    }
}
