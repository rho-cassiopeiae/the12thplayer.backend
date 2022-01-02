using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;

using Livescore.Domain.Aggregates.UserVote;
using Livescore.Domain.Base;

namespace Livescore.Infrastructure.Persistence.Repositories {
    public class UserVoteRepository : IUserVoteRepository {
        private readonly LivescoreDbContext _livescoreDbContext;

        public UserVoteRepository(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public void EnlistAsPartOf(IUnitOfWork unitOfWork) {
            _livescoreDbContext.Database.SetDbConnection(unitOfWork.Connection);
            _livescoreDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _livescoreDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Create(IEnumerable<UserVote> userVotes) {
            var userIdsParam = new NpgsqlParameter<long[]>("UserIds", NpgsqlDbType.Array | NpgsqlDbType.Bigint) {
                TypedValue = userVotes.Select(uv => uv.UserId).ToArray()
            };
            var fixtureIdParam = new NpgsqlParameter<long>("FixtureId", NpgsqlDbType.Bigint) {
                TypedValue = userVotes.First().FixtureId
            };
            var teamIdParam = new NpgsqlParameter<long>("TeamId", NpgsqlDbType.Bigint) {
                TypedValue = userVotes.First().TeamId
            };
            var playerRatingsParam = new NpgsqlParameter<IReadOnlyDictionary<string, float?>[]>(
                "PlayerRatings", NpgsqlDbType.Array | NpgsqlDbType.Jsonb
            ) {
                TypedValue = userVotes.Select(uv => uv.FixtureParticipantKeyToRating).ToArray()
            };
            var liveCommentaryVotesParam = new NpgsqlParameter<IReadOnlyDictionary<string, short?>[]>(
                "LiveCommentaryVotes", NpgsqlDbType.Array | NpgsqlDbType.Jsonb
            ) {
                TypedValue = userVotes.Select(uv => uv.LiveCommentaryAuthorIdToVote).ToArray()
            };
            var videoReactionVotesParam = new NpgsqlParameter<IReadOnlyDictionary<string, short?>[]>(
                "VideoReactionVotes", NpgsqlDbType.Array | NpgsqlDbType.Jsonb
            ) {
                TypedValue = userVotes.Select(uv => uv.VideoReactionAuthorIdToVote).ToArray()
            };

            await _livescoreDbContext.Database.ExecuteSqlRawAsync($@"
                CALL livescore.insert_or_ignore_user_vote_multi (
                    @{userIdsParam.ParameterName}, @{fixtureIdParam.ParameterName}, @{teamIdParam.ParameterName},
                    @{playerRatingsParam.ParameterName}, @{liveCommentaryVotesParam.ParameterName}, @{videoReactionVotesParam.ParameterName}
                )",
                userIdsParam, fixtureIdParam, teamIdParam, playerRatingsParam, liveCommentaryVotesParam, videoReactionVotesParam
            );
        }
    }
}
