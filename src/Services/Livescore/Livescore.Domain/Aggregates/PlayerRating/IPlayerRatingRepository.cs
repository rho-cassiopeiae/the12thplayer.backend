using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.PlayerRating {
    public interface IPlayerRatingRepository : IRepository<PlayerRating> {
        Task Create(IEnumerable<PlayerRating> playerRatings);
    }
}
