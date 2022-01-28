using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;

using MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam;
using MatchPredictions.Application.Common.Interfaces;

namespace MatchPredictions.Infrastructure.Persistence.Queryables {
    public class FixtureQueryable : IFixtureQueryable {
        private readonly MatchPredictionsDbContext _matchPredictionsDbContext;

        public FixtureQueryable(MatchPredictionsDbContext matchPredictionsDbContext) {
            _matchPredictionsDbContext = matchPredictionsDbContext;
        }

        public async Task<IEnumerable<FixtureDto>> GetAllFor(
            IEnumerable<long> seasonIds, IEnumerable<long> roundIds
        ) {
            var seasonIdsParam = new NpgsqlParameter<long[]>(nameof(seasonIds), NpgsqlDbType.Array | NpgsqlDbType.Bigint) {
                TypedValue = seasonIds.ToArray()
            };
            var roundIdsParam = new NpgsqlParameter<long[]>(nameof(roundIds), NpgsqlDbType.Array | NpgsqlDbType.Bigint) {
                TypedValue = roundIds.ToArray()
            };

            var fixtures = await _matchPredictionsDbContext.ActiveFixtures
                .FromSqlRaw(
                    $@"
                        SELECT *
                        FROM match_predictions.get_fixtures_by_season_and_round_id(
                            @{seasonIdsParam.ParameterName}, @{roundIdsParam.ParameterName}
                        )
                    ",
                    seasonIdsParam, roundIdsParam
                )
                .ToListAsync();

            return fixtures;
        }
    }
}
