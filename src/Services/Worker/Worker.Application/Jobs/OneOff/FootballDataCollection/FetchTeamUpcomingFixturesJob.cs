using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Worker.Application.Common.Interfaces;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection {
    public class FetchTeamUpcomingFixturesJob : OneOffJob {
        private readonly IFootballDataProvider _footballDataProvider;
        private readonly ILivescoreSeeder _livescoreSeeder;

        public FetchTeamUpcomingFixturesJob(
            ILogger<FetchTeamUpcomingFixturesJob> logger,
            IFootballDataProvider footballDataProvider,
            ILivescoreSeeder livescoreSeeder
        ) : base(logger) {
            _footballDataProvider = footballDataProvider;
            _livescoreSeeder = livescoreSeeder;
        }

        protected override async Task<IDictionary<string, object>> _execute() {
            var teamId = long.Parse(_context.MergedJobDataMap.GetString("TeamId"));

            var fixtures = (await _footballDataProvider.GetTeamUpcomingFixtures(teamId)).ToList();

            var seasonIds = fixtures
                .Where(fixture => fixture.SeasonId != null)
                .Select(fixture => fixture.SeasonId.Value)
                .Distinct();

            var seasons = await _footballDataProvider.GetSeasons(seasonIds);

            await _livescoreSeeder.AddTeamUpcomingFixtures(teamId, fixtures, seasons);

            return new Dictionary<string, object> {
                ["TeamId"] = teamId.ToString()
            };
        }
    }
}
