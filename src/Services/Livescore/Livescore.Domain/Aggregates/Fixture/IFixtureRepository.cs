using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Fixture {
    public interface IFixtureRepository : IRepository<Fixture> {
        Task<IEnumerable<Fixture>> FindByTeamId(long teamId);
        void Create(Fixture fixture);
    }
}
