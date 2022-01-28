using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;

using MatchPredictions.Domain.Aggregates.UserPrediction;
using MatchPredictions.Application.Common.Interfaces;

namespace MatchPredictions.Infrastructure.Persistence.Queryables {
    public class UserPredictionQueryable : IUserPredictionQueryable {
        private readonly MatchPredictionsDbContext _matchPredictionsDbContext;

        public UserPredictionQueryable(MatchPredictionsDbContext matchPredictionsDbContext) {
            _matchPredictionsDbContext = matchPredictionsDbContext;
        }

        public async Task<IEnumerable<UserPrediction>> GetAllFor(
            long userId, IEnumerable<long> seasonIds, IEnumerable<long> roundIds
        ) {
            var userIdParam = new NpgsqlParameter<long>(nameof(UserPrediction.UserId), NpgsqlDbType.Bigint) {
                TypedValue = userId
            };
            var seasonIdsParam = new NpgsqlParameter<long[]>(nameof(seasonIds), NpgsqlDbType.Array | NpgsqlDbType.Bigint) {
                TypedValue = seasonIds.ToArray()
            };
            var roundIdsParam = new NpgsqlParameter<long[]>(nameof(roundIds), NpgsqlDbType.Array | NpgsqlDbType.Bigint) {
                TypedValue = roundIds.ToArray()
            };

            var userPredictions = await _matchPredictionsDbContext.UserPredictions
                .FromSqlRaw(
                    $@"
                        SELECT *
                        FROM match_predictions.get_user_predictions_by_season_and_round_id(
                            @{userIdParam.ParameterName}, @{seasonIdsParam.ParameterName}, @{roundIdsParam.ParameterName}
                        )
                    ",
                    userIdParam, seasonIdsParam, roundIdsParam
                )
                .AsNoTracking()
                .ToListAsync();

            return userPredictions;
        }
    }
}
