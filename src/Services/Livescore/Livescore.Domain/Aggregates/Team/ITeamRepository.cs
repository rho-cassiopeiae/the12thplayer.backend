using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Team {
    public interface ITeamRepository : IRepository<Team> {
        Task<Team> FindById(long id);
        void Create(Team team);
    }
}
