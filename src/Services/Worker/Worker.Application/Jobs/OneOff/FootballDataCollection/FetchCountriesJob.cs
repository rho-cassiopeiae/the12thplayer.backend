using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Worker.Application.Common.Interfaces;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection {
    public class FetchCountriesJob : OneOffJob {
        private readonly IFootballDataProvider _footballDataProvider;

        public FetchCountriesJob(
            ILogger<FetchCountriesJob> logger,
            IFootballDataProvider footballDataProvider
        ) : base(logger) {
            _footballDataProvider = footballDataProvider;
        }

        protected override async Task<IDictionary<string, object>> _execute() {
            //var countryDtos = await _footballDataProvider.GetCountries();

            return null;
        }
    }
}
