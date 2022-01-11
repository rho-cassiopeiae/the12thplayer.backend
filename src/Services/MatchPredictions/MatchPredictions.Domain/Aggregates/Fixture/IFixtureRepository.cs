using System.Collections.Generic;
using System.Threading.Tasks;

using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.Fixture {
    public interface IFixtureRepository : IRepository<Fixture> {
        Task<IEnumerable<Fixture>> FindById(IEnumerable<long> ids);
        void Create(Fixture fixture);
    }
}
