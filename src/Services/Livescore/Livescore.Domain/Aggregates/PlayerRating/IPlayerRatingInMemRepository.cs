using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.PlayerRating {
    public interface IPlayerRatingInMemRepository : IInMemRepository<PlayerRating> {
        void CreateIfNotExists(PlayerRating playerRating);
    }
}
