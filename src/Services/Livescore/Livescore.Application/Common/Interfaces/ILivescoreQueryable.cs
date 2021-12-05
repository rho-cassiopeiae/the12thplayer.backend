using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Application.Livescore.Fixture.Common.Dto;
using Livescore.Domain.Aggregates.Player;

namespace Livescore.Application.Common.Interfaces {
    public interface ILivescoreQueryable {
        Task<IEnumerable<Player>> GetPlayersFrom(long teamId);

        Task<IEnumerable<FixtureSummaryDto>> GetFixturesForTeamInBetween(
            long teamId, long startTime, long endTime
        );
    }
}
