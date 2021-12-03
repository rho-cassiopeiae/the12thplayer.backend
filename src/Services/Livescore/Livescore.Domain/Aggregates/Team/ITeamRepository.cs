using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Team {
    public interface ITeamRepository : IRepository<Team> {
        Task<Team> FindById(long id);
        Task<IEnumerable<Team>> FindById(IEnumerable<long> ids);
        void Create(Team team);
    }
}
