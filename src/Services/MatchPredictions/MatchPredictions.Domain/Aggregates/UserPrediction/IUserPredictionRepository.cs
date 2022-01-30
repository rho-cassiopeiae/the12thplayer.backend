using System.Threading.Tasks;

using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.UserPrediction {
    public interface IUserPredictionRepository : IRepository<UserPrediction> {
        Task<UserPrediction> FindOneFor(long userId, long seasonId, long roundId);
        void Create(UserPrediction userPrediction);
    }
}
