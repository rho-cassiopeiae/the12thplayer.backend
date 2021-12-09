using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.UserVote;
using PlayerRatingDm = Livescore.Domain.Aggregates.PlayerRating.PlayerRating;

namespace Livescore.Application.Livescore.PlayerRating.Queries.GetPlayerRatingsForFixture {
    public class GetPlayerRatingsForFixtureQuery : IRequest<HandleResult<FixturePlayerRatingsDto>> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
    }

    public class GetPlayerRatingsForFixtureQueryHandler : IRequestHandler<
        GetPlayerRatingsForFixtureQuery, HandleResult<FixturePlayerRatingsDto>
    > {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IFixtureLivescoreStatusInMemQueryable _fixtureLivescoreStatusInMemQueryable;
        private readonly IPlayerRatingInMemQueryable _playerRatingInMemQueryable;
        private readonly IUserVoteInMemQueryable _userVoteInMemQueryable;

        private readonly IPlayerRatingQueryable _playerRatingQueryable;
        private readonly IUserVoteQueryable _userVoteQueryable;

        public GetPlayerRatingsForFixtureQueryHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IFixtureLivescoreStatusInMemQueryable fixtureLivescoreStatusInMemQueryable,
            IPlayerRatingInMemQueryable playerRatingInMemQueryable,
            IUserVoteInMemQueryable userVoteInMemQueryable,
            IPlayerRatingQueryable playerRatingQueryable,
            IUserVoteQueryable userVoteQueryable
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _fixtureLivescoreStatusInMemQueryable = fixtureLivescoreStatusInMemQueryable;
            _playerRatingInMemQueryable = playerRatingInMemQueryable;
            _userVoteInMemQueryable = userVoteInMemQueryable;
            _playerRatingQueryable = playerRatingQueryable;
            _userVoteQueryable = userVoteQueryable;
        }

        public async Task<HandleResult<FixturePlayerRatingsDto>> Handle(
            GetPlayerRatingsForFixtureQuery query, CancellationToken cancellationToken
        ) {
            bool active = await _fixtureLivescoreStatusInMemQueryable.CheckActive(
                query.FixtureId, query.TeamId
            );

            var ratingsFinalized = true;
            IEnumerable<PlayerRatingDm> playerRatings;
            if (active) {
                playerRatings = await _playerRatingInMemQueryable.GetAllFor(query.FixtureId, query.TeamId);
                ratingsFinalized = false;

                // @@NOTE: It's theoretically possible but highly unlikely that in between checking for active status
                // and retrieving player ratings the fixture gets finalized, and we get empty player rating list.
                // It's not worth performing some additional checks, so we just assume that the fixture is still active
                // and carry on.
            } else {
                playerRatings = await _playerRatingQueryable.GetAllFor(query.FixtureId, query.TeamId);
                if (!playerRatings.Any()) {
                    // @@NOTE: If a fixture is not active and there aren't any player ratings for it in the db,
                    // it means one of two things: either the fixture is upcoming not yet active, or
                    // the fixture is being finalized at this very moment (the process is somewhere in between setting
                    // the fixture as no longer active and saving its player ratings to the db). The latter is highly
                    // unlikely. We can perform some additional checks and stuff, but it's not worth it, since it
                    // doesn't impact user experience in all but very rare cases. So we just assume the former and carry on.

                    ratingsFinalized = false;
                }
            }

            var playerRatingsWithUserVote = playerRatings
                .Select(pr => new PlayerRatingWithUserVoteDto {
                    ParticipantKey = pr.ParticipantKey,
                    TotalRating = pr.TotalRating,
                    TotalVoters = pr.TotalVoters
                })
                .ToList();

            if (playerRatingsWithUserVote.Any() && _authenticationContext.User != null) {
                long userId = _principalDataProvider.GetId(_authenticationContext.User);

                UserVote userVote;
                if (active) {
                    userVote = await _userVoteInMemQueryable.GetPlayerRatingsFor(
                        userId, query.FixtureId, query.TeamId,
                        playerRatingsWithUserVote.Select(pr => pr.ParticipantKey).ToList()
                    );
                } else {
                    userVote = await _userVoteQueryable.GetPlayerRatingsFor(
                        userId, query.FixtureId, query.TeamId
                    );
                }

                if (userVote != null) {
                    foreach (var participantKeyToRating in userVote.FixtureParticipantKeyToRating) {
                        var playerRatingWithUserVote = playerRatingsWithUserVote.First(pr => pr.ParticipantKey == participantKeyToRating.Key);
                        playerRatingWithUserVote.UserRating = participantKeyToRating.Value.Value;
                    }
                }
            }

            return new HandleResult<FixturePlayerRatingsDto> {
                Data = new FixturePlayerRatingsDto {
                    RatingsFinalized = ratingsFinalized,
                    PlayerRatings = playerRatingsWithUserVote
                }
            };
        }
    }
}
