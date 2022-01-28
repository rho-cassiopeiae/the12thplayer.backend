using System.Collections.Generic;
using System.Threading.Tasks;

using MatchPredictions.Domain.Aggregates.UserPrediction;

namespace MatchPredictions.Application.Common.Interfaces {
    public interface IUserPredictionQueryable {
        Task<IEnumerable<UserPrediction>> GetAllFor(
            long userId, IEnumerable<long> seasonIds, IEnumerable<long> roundIds
        );
    }
}
