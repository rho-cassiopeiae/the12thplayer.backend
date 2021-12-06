using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.PlayerRating {
    public interface IPlayerRatingInMemRepository : IInMemRepository<PlayerRating> {
        void CreateIfNotExists(PlayerRating playerRating);

        Task<PlayerRating> FindUserVoteForPlayer(
            long fixtureId, long teamId, long userId, string participantKey
        );

        void Update(PlayerRating playerRating);
    }
}
