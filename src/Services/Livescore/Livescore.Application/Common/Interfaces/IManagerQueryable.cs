using System.Threading.Tasks;

using Livescore.Application.Team.Queries.GetTeamSquad;

namespace Livescore.Application.Common.Interfaces {
    public interface IManagerQueryable {
        Task<ManagerDto> GetManagerWithCountryFrom(long teamId);
    }
}
