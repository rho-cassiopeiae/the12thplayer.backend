using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;

using MatchPredictions.Domain.Aggregates.Team;
using MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam;
using MatchPredictions.Application.Common.Interfaces;

namespace MatchPredictions.Infrastructure.Persistence.Queryables {
    public class TeamActiveSeasonsQueryable : ITeamActiveSeasonsQueryable {
        private readonly MatchPredictionsDbContext _matchPredictionsDbContext;

        public TeamActiveSeasonsQueryable(MatchPredictionsDbContext matchPredictionsDbContext) {
            _matchPredictionsDbContext = matchPredictionsDbContext;
        }

        public async Task<IEnumerable<ActiveSeasonRoundWithFixturesDto>> GetAllWithActiveRoundsForTeam(
            long teamId
        ) {
            var teamIdParam = new NpgsqlParameter<long>(nameof(TeamActiveSeasons.TeamId), NpgsqlDbType.Bigint) {
                TypedValue = teamId
            };

            var seasonRounds = await _matchPredictionsDbContext.ActiveSeasonRounds
                .FromSqlRaw(
                    $@"
                        SELECT *
                        FROM match_predictions.get_active_seasons_with_active_rounds_for_team (@{teamIdParam.ParameterName})
                    ",
                    teamIdParam
                )
                .ToListAsync();

            return seasonRounds;
        }
    }
}
