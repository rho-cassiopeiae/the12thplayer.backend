using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Application.Livescore.Fixture.Common.Dto;

namespace Livescore.Application.Common.Interfaces {
    public interface IFixtureQueryable {
        Task<IEnumerable<FixtureSummaryDto>> GetFixturesForTeamInBetween(
            long teamId, long startTime, long endTime
        );

        Task<FixtureFullDto> GetFixtureForTeam(long fixtureId, long teamId);
    }
}
