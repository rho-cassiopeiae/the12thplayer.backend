using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Application.Team.Queries.GetTeamSquad;
using Livescore.Domain.Aggregates.Player;

namespace Livescore.Application.Common.Interfaces {
    public interface IPlayerQueryable {
        Task<IEnumerable<Player>> GetPlayersFrom(long teamId);
        Task<IEnumerable<PlayerDto>> GetPlayersWithCountryFrom(long teamId);
    }
}
