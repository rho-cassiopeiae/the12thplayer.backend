using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Worker.Application.Common.Interfaces;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection {
    public class FetchSeasonWithRoundsAndFixturesJob : OneOffJob {
        private readonly IFootballDataProvider _footballDataProvider;
        private readonly IMatchPredictionsSeeder _matchPredictionsSeeder;
        
        public FetchSeasonWithRoundsAndFixturesJob(
            ILogger<FetchSeasonWithRoundsAndFixturesJob> logger,
            IFootballDataProvider footballDataProvider,
            IMatchPredictionsSeeder matchPredictionsSeeder
        ) : base(logger) {
            _footballDataProvider = footballDataProvider;
            _matchPredictionsSeeder = matchPredictionsSeeder;
        }

        protected override async Task<IDictionary<string, object>> _execute() {
            var seasonId = long.Parse(_context.MergedJobDataMap.GetString("SeasonId"));

            var season = await _footballDataProvider.GetSeasonWithRoundsAndFixtures(seasonId);

            await _matchPredictionsSeeder.AddSeasonWithRoundsAndFixtures(season);

            return null;
        }
    }
}
