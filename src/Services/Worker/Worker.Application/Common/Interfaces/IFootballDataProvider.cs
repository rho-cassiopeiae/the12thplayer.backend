using System.Collections.Generic;
using System.Threading.Tasks;

using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Application.Common.Interfaces {
    public interface IFootballDataProvider {
        Task<IEnumerable<CountryDto>> GetCountries();

        Task<TeamDto> GetTeamDetails(long teamId);

        Task<IEnumerable<SeasonDto>> GetSeasons(IEnumerable<long> seasonIds);

        Task<IEnumerable<PlayerDto>> GetPlayers(IEnumerable<long> playerIds);

        Task<IEnumerable<FixtureDto>> GetTeamFinishedFixtures(
            long teamId, string startDate, string endDate
        );

        Task<IEnumerable<FixtureDto>> GetTeamUpcomingFixtures(long teamId);

        Task<FixtureDto> GetFixtureLivescore(
            long fixtureId,
            long teamId,
            bool emulateOngoing,
            bool includeReferee = false,
            bool includeLineups = false,
            bool includeEventsAndStats = false
        );
    }
}
