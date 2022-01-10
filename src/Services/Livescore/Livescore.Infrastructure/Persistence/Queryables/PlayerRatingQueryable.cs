using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;

using Livescore.Domain.Aggregates.PlayerRating;
using Livescore.Application.Common.Interfaces;
using Livescore.Application.Team.Queries.GetPlayerRatingsForParticipant;

namespace Livescore.Infrastructure.Persistence.Queryables {
    public class PlayerRatingQueryable : IPlayerRatingQueryable {
        private readonly LivescoreDbContext _livescoreDbContext;

        public PlayerRatingQueryable(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task<IEnumerable<PlayerRating>> GetAllFor(long fixtureId, long teamId) {
            var playerRatings = await _livescoreDbContext.PlayerRatings
                .Where(pr => pr.FixtureId == fixtureId && pr.TeamId == teamId)
                .Select(pr => new PlayerRating(
                    fixtureId, teamId, pr.ParticipantKey, pr.TotalRating, pr.TotalVoters
                ))
                .ToListAsync();

            return playerRatings;
        }

        public async Task<IEnumerable<FixturePlayerRatingDto>> GetAllFor(long teamId, string[] participantKeys) {
            var teamIdParam = new NpgsqlParameter<long>(nameof(PlayerRating.TeamId), NpgsqlDbType.Bigint) {
                TypedValue = teamId
            };
            var participantKeysParam = new NpgsqlParameter<string[]>(
                nameof(PlayerRating.ParticipantKey), NpgsqlDbType.Array | NpgsqlDbType.Text
            ) {
                TypedValue = participantKeys
            };

            var playerRatings = await _livescoreDbContext.FixturePlayerRatings
                .FromSqlRaw(
                    $@"
                        SELECT *
                        FROM livescore.get_player_ratings_for_participant (
                            @{teamIdParam.ParameterName}, @{participantKeysParam.ParameterName}
                        );
                    ",
                    teamIdParam, participantKeysParam
                )
                .ToListAsync();

            return playerRatings;
        }
    }
}
