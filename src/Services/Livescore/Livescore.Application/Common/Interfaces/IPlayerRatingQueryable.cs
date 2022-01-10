using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Application.Team.Queries.GetPlayerRatingsForParticipant;
using Livescore.Domain.Aggregates.PlayerRating;

namespace Livescore.Application.Common.Interfaces {
    public interface IPlayerRatingQueryable {
        Task<IEnumerable<PlayerRating>> GetAllFor(long fixtureId, long teamId);
        Task<IEnumerable<FixturePlayerRatingDto>> GetAllFor(long teamId, string[] participantKeys);
    }
}
