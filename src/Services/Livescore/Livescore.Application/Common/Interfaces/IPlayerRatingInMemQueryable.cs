using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Aggregates.PlayerRating;

namespace Livescore.Application.Common.Interfaces {
    public interface IPlayerRatingInMemQueryable {
        Task<PlayerRating> GetRatingFor(long fixtureId, long teamId, string participantKey);
        Task<IEnumerable<PlayerRating>> GetAllFor(long fixtureId, long teamId);
    }
}
