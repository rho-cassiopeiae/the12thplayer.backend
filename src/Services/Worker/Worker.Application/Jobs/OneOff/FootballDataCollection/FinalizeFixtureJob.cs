using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection {
    public class FinalizeFixtureJob : OneOffJob {
        public FinalizeFixtureJob(
            ILogger<FinalizeFixtureJob> logger
        ) : base(logger) { }

        protected override Task<IDictionary<string, object>> _execute() {
            throw new NotImplementedException();
        }
    }
}
