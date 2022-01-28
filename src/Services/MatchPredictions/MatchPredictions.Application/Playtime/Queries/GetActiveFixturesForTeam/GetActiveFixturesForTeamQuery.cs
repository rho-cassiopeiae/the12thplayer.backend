using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using MatchPredictions.Application.Common.Interfaces;
using MatchPredictions.Application.Common.Results;

namespace MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam {
    public class GetActiveFixturesForTeamQuery : IRequest<
        HandleResult<IEnumerable<ActiveSeasonRoundWithFixturesDto>>
    > {
        public long TeamId { get; set; }
    }

    public class GetActiveFixturesForTeamQueryHandler : IRequestHandler<
        GetActiveFixturesForTeamQuery, HandleResult<IEnumerable<ActiveSeasonRoundWithFixturesDto>>
    > {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly ITeamActiveSeasonsQueryable _teamActiveSeasonsQueryable;
        private readonly IFixtureQueryable _fixtureQueryable;
        private readonly IUserPredictionQueryable _userPredictionQueryable;

        public GetActiveFixturesForTeamQueryHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            ITeamActiveSeasonsQueryable teamActiveSeasonsQueryable,
            IFixtureQueryable fixtureQueryable,
            IUserPredictionQueryable userPredictionQueryable
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _teamActiveSeasonsQueryable = teamActiveSeasonsQueryable;
            _fixtureQueryable = fixtureQueryable;
            _userPredictionQueryable = userPredictionQueryable;
        }

        public async Task<HandleResult<IEnumerable<ActiveSeasonRoundWithFixturesDto>>> Handle(
            GetActiveFixturesForTeamQuery query, CancellationToken cancellationToken
        ) {
            var seasonRounds = await _teamActiveSeasonsQueryable.GetAllWithActiveRoundsForTeam(query.TeamId);

            // @@!!: Must always be exactly one active round per active season. So when we change a team's active seasons,
            // should also set active rounds for them in the same transaction. And changing active round for a season should
            // be transactional as well.

            var fixtures = await _fixtureQueryable.GetAllFor(
                seasonRounds.Select(sr => sr.SeasonId),
                seasonRounds.Select(sr => sr.RoundId)
            );

            foreach (var seasonRound in seasonRounds) {
                seasonRound.Fixtures = fixtures
                    .Where(f => f.SeasonId == seasonRound.SeasonId && f.RoundId == seasonRound.RoundId)
                    .ToList();
            }

            if (seasonRounds.Any() && _authenticationContext.User != null) {
                var userPredictions = await _userPredictionQueryable.GetAllFor(
                    _principalDataProvider.GetId(_authenticationContext.User),
                    seasonRounds.Select(sr => sr.SeasonId),
                    seasonRounds.Select(sr => sr.RoundId)
                );

                foreach (var userPrediction in userPredictions) {
                    var roundFixtures = seasonRounds
                        .First(sr => sr.SeasonId == userPrediction.SeasonId && sr.RoundId == userPrediction.RoundId)
                        .Fixtures;

                    foreach (var fixtureIdToScore in userPrediction.FixtureIdToScore) {
                        var fixtureId = long.Parse(fixtureIdToScore.Key);
                        var fixture = roundFixtures.First(f => f.Id == fixtureId);
                        var predictedScoreSplit = fixtureIdToScore.Value.Split('-');
                        fixture.PredictedHomeTeamScore = short.Parse(predictedScoreSplit.First());
                        fixture.PredictedGuestTeamScore = short.Parse(predictedScoreSplit.Last());
                    }
                }
            }

            return new HandleResult<IEnumerable<ActiveSeasonRoundWithFixturesDto>> {
                Data = seasonRounds
            };
        }
    }
}
