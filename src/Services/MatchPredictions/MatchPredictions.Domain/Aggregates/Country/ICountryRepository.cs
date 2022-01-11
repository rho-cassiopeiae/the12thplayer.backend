using System.Collections.Generic;

using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.Country {
    public interface ICountryRepository : IRepository<Country> {
        void Create(IEnumerable<Country> countries);
    }
}
