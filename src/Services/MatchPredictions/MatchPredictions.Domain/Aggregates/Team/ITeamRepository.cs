using System.Collections.Generic;
using System.Threading.Tasks;

using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.Team {
    public interface ITeamRepository : IRepository<Team> {
        Task<IEnumerable<Team>> FindById(IEnumerable<long> ids);
        void Create(Team team);
    }
}
