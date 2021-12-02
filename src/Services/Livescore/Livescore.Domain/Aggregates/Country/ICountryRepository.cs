using System.Collections.Generic;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Country {
    public interface ICountryRepository : IRepository<Country> {
        void Create(IEnumerable<Country> countries);
    }
}
