using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Player {
    public interface IPlayerRepository : IRepository<Player> {
        Task<IEnumerable<Player>> FindById(IEnumerable<long> ids);
        void Create(Player player);
    }
}
