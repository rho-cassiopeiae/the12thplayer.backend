using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Aggregates.Player;

namespace Livescore.Application.Common.Interfaces {
    public interface ILivescoreQueryable {
        Task<IEnumerable<Player>> GetPlayersFrom(long teamId);
    }
}
