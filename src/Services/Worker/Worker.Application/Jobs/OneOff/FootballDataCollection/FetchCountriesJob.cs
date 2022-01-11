using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Worker.Application.Common.Interfaces;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection {
    public class FetchCountriesJob : OneOffJob {
        private readonly IFootballDataProvider _footballDataProvider;
        private readonly ILivescoreSeeder _livescoreSeeder;
        private readonly IMatchPredictionsSeeder _matchPredictionsSeeder;

        public FetchCountriesJob(
            ILogger<FetchCountriesJob> logger,
            IFootballDataProvider footballDataProvider,
            ILivescoreSeeder livescoreSeeder,
            IMatchPredictionsSeeder matchPredictionsSeeder
        ) : base(logger) {
            _footballDataProvider = footballDataProvider;
            _livescoreSeeder = livescoreSeeder;
            _matchPredictionsSeeder = matchPredictionsSeeder;
        }

        protected override async Task<IDictionary<string, object>> _execute() {
            var countries = await _footballDataProvider.GetCountries();

            await _livescoreSeeder.AddCountries(countries);
            await _matchPredictionsSeeder.AddCountries(countries);

            return null;
        }
    }
}
