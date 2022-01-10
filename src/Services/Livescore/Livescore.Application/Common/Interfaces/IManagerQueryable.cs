using System.Threading.Tasks;

using Livescore.Application.Team.Queries.GetTeamSquad.Dto;

namespace Livescore.Application.Common.Interfaces {
    public interface IManagerQueryable {
        Task<ManagerDto> GetManagerWithCountryFrom(long teamId);
    }
}
