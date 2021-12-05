using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;

using Livescore.Domain.Aggregates.Player;
using Livescore.Application.Common.Interfaces;
using Livescore.Application.Livescore.Fixture.Common.Dto;

namespace Livescore.Infrastructure.Persistence.Queryables {
    public class LivescoreQueryable : ILivescoreQueryable {
        private readonly LivescoreDbContext _livescoreDbContext;

        public LivescoreQueryable(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task<IEnumerable<Player>> GetPlayersFrom(long teamId) {
            var players = await _livescoreDbContext.Players
                .AsNoTracking()
                .Where(p => p.TeamId == teamId)
                .ToListAsync();

            return players;
        }

        public async Task<IEnumerable<FixtureSummaryDto>> GetFixturesForTeamInBetween(
            long teamId, long startTime, long endTime
        ) {
            var teamIdParameter = new NpgsqlParameter<long>("TeamId", teamId);
            var startTimeParameter = new NpgsqlParameter<long>("StartTime", startTime);
            var endTimeParameter = new NpgsqlParameter<long>("EndTime", endTime);

            var fixtures = await _livescoreDbContext.FixtureSummaries
                .FromSqlRaw(
                    $"SELECT * FROM livescore.get_fixtures_for_team_in_between (@{teamIdParameter.ParameterName}, @{startTimeParameter.ParameterName}, @{endTimeParameter.ParameterName})",
                    teamIdParameter, startTimeParameter, endTimeParameter
                )
                .ToListAsync();

            return fixtures;
        }
    }
}
