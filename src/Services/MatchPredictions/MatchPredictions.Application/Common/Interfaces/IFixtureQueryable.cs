using System.Collections.Generic;
using System.Threading.Tasks;

using MatchPredictions.Application.Playtime.Commands.SubmitMatchPredictions;
using MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam;

namespace MatchPredictions.Application.Common.Interfaces {
    public interface IFixtureQueryable {
        Task<IEnumerable<FixtureDto>> GetAllFor(IEnumerable<long> seasonIds, IEnumerable<long> roundIds);
        Task<IEnumerable<NotStartedFixtureDto>> GetOnlyNotStarted(IEnumerable<long> fixtureIds);
        Task<IEnumerable<AlreadyStartedFixtureDto>> GetById(IEnumerable<long> fixtureIds);
    }
}
