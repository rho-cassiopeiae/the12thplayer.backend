using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Worker.Application.Common.Interfaces;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection {
    public class FinalizeFixtureJob : OneOffJob {
        private readonly IFixtureLivescoreNotifier _fixtureLivescoreNotifier;
        
        public FinalizeFixtureJob(
            ILogger<FinalizeFixtureJob> logger,
            IFixtureLivescoreNotifier fixtureLivescoreNotifier
        ) : base(logger) {
            _fixtureLivescoreNotifier = fixtureLivescoreNotifier;
        }

        protected override async Task<IDictionary<string, object>> _execute() {
            var fixtureId = long.Parse(_context.MergedJobDataMap.GetString("FixtureId"));
            var teamId = long.Parse(_context.MergedJobDataMap.GetString("TeamId"));

            await _fixtureLivescoreNotifier.NotifyFixtureDeactivated(fixtureId, teamId);

            return null;
        }
    }
}
