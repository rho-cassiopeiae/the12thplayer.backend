using System.Collections.Generic;
using System.Threading.Tasks;

using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Application.Common.Interfaces {
    public interface IFootballDataProvider {
        Task<IEnumerable<CountryDto>> GetCountries();
        Task<TeamDto> GetTeamById(long teamId);

        //Task<IEnumerable<FixtureDto>> GetTeamFinishedFixtures(
        //    long teamId, string startDate, string endDate
        //);

        //Task<IEnumerable<FixtureDto>> GetTeamUpcomingFixtures(long teamId);

        //Task<IEnumerable<PlayerResponseDto.PlayerDto>> GetPlayersByIds(IEnumerable<long> playerIds);

        //Task<FixtureDto> GetFixtureLivescore(
        //    long fixtureId,
        //    bool emulateOngoing,
        //    bool includeReferee = false,
        //    bool includeLineups = false,
        //    bool includeEventsAndStats = false
        //);
    }
}
