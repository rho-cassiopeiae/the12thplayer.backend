using System.Collections.Generic;
using System.Threading.Tasks;

using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Application.Common.Interfaces {
    public interface ILivescoreSeeder {
        Task AddCountries(IEnumerable<CountryDto> countries);
        Task AddTeamDetails(TeamDto team);
    }
}
