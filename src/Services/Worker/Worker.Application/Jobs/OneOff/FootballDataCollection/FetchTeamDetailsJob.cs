using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Worker.Application.Common.Interfaces;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection {
    public class FetchTeamDetailsJob : OneOffJob {
        private readonly IFootballDataProvider _footballDataProvider;

        public FetchTeamDetailsJob(
            ILogger<FetchTeamDetailsJob> logger,
            IFootballDataProvider footballDataProvider
        ) : base(logger) {
            _footballDataProvider = footballDataProvider;
        }

        protected override async Task<IDictionary<string, object>> _execute() {
            var teamId = long.Parse(_context.MergedJobDataMap.GetString("TeamId"));

            //var teamDto = await _footballDataProvider.GetTeamById(teamId);

            return new Dictionary<string, object> {
                ["TeamId"] = teamId
            };
        }
    }
}
