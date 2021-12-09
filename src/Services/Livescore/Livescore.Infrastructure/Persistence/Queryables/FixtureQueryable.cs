using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Livescore.Fixture.Common.Dto;

namespace Livescore.Infrastructure.Persistence.Queryables {
    public class FixtureQueryable : IFixtureQueryable {
        private readonly LivescoreDbContext _livescoreDbContext;

        public FixtureQueryable(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
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

        public async Task<FixtureFullDto> GetFixtureForTeam(long fixtureId, long teamId) {
            var fixtureIdParameter = new NpgsqlParameter<long>("FixtureId", fixtureId);
            var teamIdParameter = new NpgsqlParameter<long>("TeamId", teamId);

            var fixture = await _livescoreDbContext.FixtureFullViews
                .FromSqlRaw(
                    $"SELECT * FROM livescore.get_fixture_for_team (@{fixtureIdParameter.ParameterName}, @{teamIdParameter.ParameterName})",
                    fixtureIdParameter, teamIdParameter
                )
                .SingleAsync();

            return fixture;
        }
    }
}
