using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.PlayerRating {
    public interface IPlayerRatingInMemRepository : IInMemRepository<PlayerRating> {
        Task<IEnumerable<PlayerRating>> FindAllFor(long fixtureId, long teamId);

        void CreateIfNotExists(PlayerRating playerRating);

        void UpdateOneFor(
            long fixtureId, long teamId, string participantKey,
            float? oldRating, float newRating
        );

        void DeleteAllFor(long fixtureId, long teamId);
    }
}
