using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;

using Livescore.Domain.Aggregates.PlayerRating;
using Livescore.Domain.Base;

namespace Livescore.Infrastructure.Persistence.Repositories {
    public class PlayerRatingRepository : IPlayerRatingRepository {
        private readonly LivescoreDbContext _livescoreDbContext;

        private IUnitOfWork _unitOfWork;

        public PlayerRatingRepository(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public void EnlistAsPartOf(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _livescoreDbContext.Database.SetDbConnection(unitOfWork.Connection);
            _livescoreDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _livescoreDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Create(IEnumerable<PlayerRating> playerRatings) {
            var fixtureIdParam = new NpgsqlParameter<long>("FixtureId", NpgsqlDbType.Bigint) {
                TypedValue = playerRatings.First().FixtureId
            };
            var teamIdParam = new NpgsqlParameter<long>("TeamId", NpgsqlDbType.Bigint) {
                TypedValue = playerRatings.First().TeamId
            };
            var participantKeysParam = new NpgsqlParameter<string[]>("ParticipantKeys", NpgsqlDbType.Array | NpgsqlDbType.Text) {
                TypedValue = playerRatings.Select(pr => pr.ParticipantKey).ToArray()
            };
            var totalRatingsParam = new NpgsqlParameter<int[]>("TotalRatings", NpgsqlDbType.Array | NpgsqlDbType.Integer) {
                TypedValue = playerRatings.Select(pr => pr.TotalRating).ToArray()
            };
            var totalVotersParam = new NpgsqlParameter<int[]>("TotalVoters", NpgsqlDbType.Array | NpgsqlDbType.Integer) {
                TypedValue = playerRatings.Select(pr => pr.TotalVoters).ToArray()
            };

            await _livescoreDbContext.Database.ExecuteSqlRawAsync($@"
                CALL livescore.insert_or_ignore_player_rating_multi (
                    @{fixtureIdParam.ParameterName}, @{teamIdParam.ParameterName}, @{participantKeysParam.ParameterName},
                    @{totalRatingsParam.ParameterName}, @{totalVotersParam.ParameterName}
                )",
                fixtureIdParam, teamIdParam, participantKeysParam, totalRatingsParam, totalVotersParam
            );
        }
    }
}
