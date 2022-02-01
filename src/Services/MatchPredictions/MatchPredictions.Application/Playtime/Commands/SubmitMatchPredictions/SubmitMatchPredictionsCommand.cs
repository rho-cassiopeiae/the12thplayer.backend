using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using MatchPredictions.Application.Common.Attributes;
using MatchPredictions.Application.Common.Interfaces;
using MatchPredictions.Application.Common.Results;
using MatchPredictions.Domain.Aggregates.UserPrediction;
using MatchPredictions.Domain.Base;

namespace MatchPredictions.Application.Playtime.Commands.SubmitMatchPredictions {
    [RequireAuthorization]
    public class SubmitMatchPredictionsCommand : IRequest<HandleResult<MatchPredictionsSubmissionDto>> {
        public long SeasonId { get; init; }
        public long RoundId { get; init; }
        public Dictionary<string, string> FixtureIdToScore { get; init; }
    }

    public class SubmitMatchPredictionsCommandHandler : IRequestHandler<
        SubmitMatchPredictionsCommand, HandleResult<MatchPredictionsSubmissionDto>
    > {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserPredictionRepository _userPredictionRepository;
        private readonly IFixtureQueryable _fixtureQueryable;

        public SubmitMatchPredictionsCommandHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IUnitOfWork unitOfWork,
            IUserPredictionRepository userPredictionRepository,
            IFixtureQueryable fixtureQueryable
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _unitOfWork = unitOfWork;
            _userPredictionRepository = userPredictionRepository;
            _fixtureQueryable = fixtureQueryable;
        }

        public async Task<HandleResult<MatchPredictionsSubmissionDto>> Handle(
            SubmitMatchPredictionsCommand command, CancellationToken cancellationToken
        ) {
            long userId = _principalDataProvider.GetId(_authenticationContext.User);

            await _unitOfWork.Begin(IsolationLevel.ReadCommitted);

            _userPredictionRepository.EnlistAsPartOf(_unitOfWork);

            var userPrediction = await _userPredictionRepository.FindOneFor(
                userId, command.SeasonId, command.RoundId
            );

            Dictionary<string, string> newPredictions;
            if (userPrediction != null) {
                newPredictions = new();
                foreach (var prediction in command.FixtureIdToScore) {
                    var fixtureId = prediction.Key;
                    var isNewPrediction = true;
                    // @@NOTE: userPrediction.FixtureIdToScore can't be null here, since it's configured as non-nullable in the db.
                    if (userPrediction.FixtureIdToScore.TryGetValue(fixtureId, out string currentPrediction)) {
                        if (prediction.Value == currentPrediction) {
                            isNewPrediction = false;
                        }
                    }

                    if (isNewPrediction) {
                        newPredictions[fixtureId] = prediction.Value;
                    }
                }
            } else {
                newPredictions = command.FixtureIdToScore;
            }

            IEnumerable<AlreadyStartedFixtureDto> alreadyStartedFixtures = null;

            if (newPredictions.Any()) {
                var notStartedFixtures = await _fixtureQueryable.GetOnlyNotStarted(
                    newPredictions.Keys.Select(fixtureId => long.Parse(fixtureId))
                ); // @@NOTE: This also filters out non-existent fixtures.

                var notStartedFixtureIds = notStartedFixtures.Select(f => f.Id.ToString()).ToHashSet();
                var newPredictionsForNotStartedFixtures = new Dictionary<string, string>(
                    newPredictions.Where(p => notStartedFixtureIds.Contains(p.Key))
                );

                if (newPredictionsForNotStartedFixtures.Any()) {
                    if (userPrediction == null) {
                        userPrediction = new UserPrediction(
                            userId: userId,
                            seasonId: command.SeasonId,
                            roundId: command.RoundId
                        );
                        userPrediction.SetPredictions(newPredictionsForNotStartedFixtures);

                        _userPredictionRepository.Create(userPrediction);
                    } else {
                        userPrediction.AddPredictions(newPredictionsForNotStartedFixtures);
                    }

                    await _userPredictionRepository.SaveChanges(cancellationToken);
                }

                var newPredictionsForAlreadyStartedFixtures = newPredictions.Where(
                    p => !notStartedFixtureIds.Contains(p.Key)
                );
                if (newPredictionsForAlreadyStartedFixtures.Any()) {
                    alreadyStartedFixtures = await _fixtureQueryable.GetById(
                        newPredictionsForAlreadyStartedFixtures.Select(p => long.Parse(p.Key)).ToList()
                    );
                }
            }

            await _unitOfWork.Commit();

            return new HandleResult<MatchPredictionsSubmissionDto> {
                Data = new MatchPredictionsSubmissionDto {
                    UpdatedPredictions = userPrediction?.FixtureIdToScore,
                    AlreadyStartedFixtures = alreadyStartedFixtures
                }
            };
        }
    }
}
