using System.Collections.Generic;
using System.Threading.Tasks;

using MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam;

namespace MatchPredictions.Application.Common.Interfaces {
    public interface ITeamActiveSeasonsQueryable {
        Task<IEnumerable<ActiveSeasonRoundWithFixturesDto>> GetAllWithActiveRoundsForTeam(long teamId);
    }
}
