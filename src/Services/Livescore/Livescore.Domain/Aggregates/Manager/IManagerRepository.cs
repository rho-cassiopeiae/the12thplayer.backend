using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Manager {
    public interface IManagerRepository : IRepository<Manager> {
        Task<Manager> FindById(long id);
        void Create(Manager manager);
    }
}
