using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Worker.Application.Common.Interfaces;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection {
    public class FetchTeamDetailsJob : OneOffJob {
        private readonly IFootballDataProvider _footballDataProvider;
        private readonly ILivescoreSeeder _livescoreSeeder;

        public FetchTeamDetailsJob(
            ILogger<FetchTeamDetailsJob> logger,
            IFootballDataProvider footballDataProvider,
            ILivescoreSeeder livescoreSeeder
        ) : base(logger) {
            _footballDataProvider = footballDataProvider;
            _livescoreSeeder = livescoreSeeder;
        }

        protected override async Task<IDictionary<string, object>> _execute() {
            var teamId = long.Parse(_context.MergedJobDataMap.GetString("TeamId"));

            var team = await _footballDataProvider.GetTeamById(teamId);

            await _livescoreSeeder.AddTeamDetails(team);

            return new Dictionary<string, object> {
                ["TeamId"] = teamId.ToString()
            };
        }
    }
}
