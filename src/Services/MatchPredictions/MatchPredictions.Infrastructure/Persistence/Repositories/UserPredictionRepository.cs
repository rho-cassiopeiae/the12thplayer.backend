using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;

using MatchPredictions.Domain.Aggregates.UserPrediction;
using MatchPredictions.Domain.Base;

namespace MatchPredictions.Infrastructure.Persistence.Repositories {
    public class UserPredictionRepository : IUserPredictionRepository {
        private readonly MatchPredictionsDbContext _matchPredictionsDbContext;

        private IUnitOfWork _unitOfWork;

        public UserPredictionRepository(MatchPredictionsDbContext matchPredictionsDbContext) {
            _matchPredictionsDbContext = matchPredictionsDbContext;
        }

        public void EnlistAsPartOf(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _matchPredictionsDbContext.Database.SetDbConnection(unitOfWork.Connection);
            _matchPredictionsDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _matchPredictionsDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserPrediction> FindOneFor(long userId, long seasonId, long roundId) {
            var userIdParam = new NpgsqlParameter<long>(nameof(UserPrediction.UserId), NpgsqlDbType.Bigint) {
                TypedValue = userId
            };
            var seasonIdParam = new NpgsqlParameter<long>(nameof(UserPrediction.SeasonId), NpgsqlDbType.Bigint) {
                TypedValue = seasonId
            };
            var roundIdParam = new NpgsqlParameter<long>(nameof(UserPrediction.RoundId), NpgsqlDbType.Bigint) {
                TypedValue = roundId
            };

            var userPrediction = await _matchPredictionsDbContext.UserPredictions
                .FromSqlRaw(
                    $@"
                        SELECT *
                        FROM match_predictions.lock_and_get_user_prediction(
                            @{userIdParam.ParameterName}, @{seasonIdParam.ParameterName}, @{roundIdParam.ParameterName}
                        )
                    ",
                    userIdParam, seasonIdParam, roundIdParam
                )
                .SingleOrDefaultAsync();

            return userPrediction;
        }

        public void Create(UserPrediction userPrediction) {
            _matchPredictionsDbContext.UserPredictions.Add(userPrediction);
        }
    }
}
