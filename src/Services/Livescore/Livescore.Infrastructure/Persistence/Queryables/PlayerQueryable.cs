using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;

using Livescore.Domain.Aggregates.Player;
using Livescore.Application.Common.Interfaces;
using Livescore.Application.Team.Queries.GetTeamSquad;

namespace Livescore.Infrastructure.Persistence.Queryables {
    public class PlayerQueryable : IPlayerQueryable {
        private readonly LivescoreDbContext _livescoreDbContext;

        public PlayerQueryable(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task<IEnumerable<Player>> GetPlayersFrom(long teamId) {
            var players = await _livescoreDbContext.Players
                .AsNoTracking()
                .Where(p => p.TeamId == teamId)
                .ToListAsync();

            return players;
        }

        public async Task<IEnumerable<PlayerDto>> GetPlayersWithCountryFrom(long teamId) {
            var teamIdParam = new NpgsqlParameter<long>(nameof(Player.TeamId), NpgsqlDbType.Bigint) {
                TypedValue = teamId
            };

            var players = await _livescoreDbContext.PlayersWithCountry
                .FromSqlRaw(
                    $@"
                        SELECT *
                        FROM livescore.get_players_with_country_from (@{teamIdParam.ParameterName});
                    ",
                    teamIdParam
                )
                .ToListAsync();

            return players;
        }
    }
}
