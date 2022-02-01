using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;

using Livescore.Application.Team.Queries.GetTeamSquad;
using Livescore.Domain.Aggregates.Manager;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.Persistence.Queryables {
    public class ManagerQueryable : IManagerQueryable {
        private readonly LivescoreDbContext _livescoreDbContext;

        public ManagerQueryable(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task<ManagerDto> GetManagerWithCountryFrom(long teamId) {
            var teamIdParam = new NpgsqlParameter<long>(nameof(Manager.TeamId), NpgsqlDbType.Bigint) {
                TypedValue = teamId
            };

            var manager = await _livescoreDbContext.ManagersWithCountry
                .FromSqlRaw(
                    $@"
                        SELECT *
                        FROM livescore.get_manager_with_country_from (@{teamIdParam.ParameterName})
                    ",
                    teamIdParam
                )
                .SingleOrDefaultAsync();

            return manager;
        }
    }
}
