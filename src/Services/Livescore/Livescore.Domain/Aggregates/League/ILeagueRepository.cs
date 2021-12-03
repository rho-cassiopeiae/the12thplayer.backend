using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.League {
    public interface ILeagueRepository : IRepository<League> {
        Task<IEnumerable<League>> FindById(IEnumerable<long> ids);
        void Create(League league);
    }
}
