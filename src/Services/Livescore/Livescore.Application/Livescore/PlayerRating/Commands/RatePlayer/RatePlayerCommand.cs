using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.Common.Errors;
using Livescore.Domain.Aggregates.FixtureLivescoreStatus;
using Livescore.Domain.Aggregates.PlayerRating;
using Livescore.Domain.Aggregates.UserVote;
using Livescore.Domain.Base;
using Livescore.Application.Common.Attributes;

namespace Livescore.Application.Livescore.PlayerRating.Commands.RatePlayer {
    [RequireAuthorization]
    public class RatePlayerCommand : IRequest<HandleResult<PlayerRatingDto>> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public string ParticipantKey { get; set; }
        public float Rating { get; set; }
    }

    public class RatePlayerCommandHandler : IRequestHandler<
        RatePlayerCommand, HandleResult<PlayerRatingDto>
    > {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;
        private readonly IInMemUnitOfWork _unitOfWork;
        private readonly IFixtureLivescoreStatusInMemRepository _fixtureLivescoreStatusInMemRepository;
        private readonly IUserVoteInMemRepository _userVoteInMemRepository;
        private readonly IPlayerRatingInMemRepository _playerRatingInMemRepository;
        private readonly IPlayerRatingInMemQueryable _playerRatingInMemQueryable;

        public RatePlayerCommandHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IInMemUnitOfWork unitOfWork,
            IFixtureLivescoreStatusInMemRepository fixtureLivescoreStatusInMemRepository,
            IUserVoteInMemRepository userVoteInMemRepository,
            IPlayerRatingInMemRepository playerRatingInMemRepository,
            IPlayerRatingInMemQueryable playerRatingInMemQueryable
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _unitOfWork = unitOfWork;
            _fixtureLivescoreStatusInMemRepository = fixtureLivescoreStatusInMemRepository;
            _userVoteInMemRepository = userVoteInMemRepository;
            _playerRatingInMemRepository = playerRatingInMemRepository;
            _playerRatingInMemQueryable = playerRatingInMemQueryable;
        }

        public async Task<HandleResult<PlayerRatingDto>> Handle(
            RatePlayerCommand command, CancellationToken cancellationToken
        ) {
            long userId = _principalDataProvider.GetId(_authenticationContext.User);

            bool applied;
            do {
                var active = await _fixtureLivescoreStatusInMemRepository.FindOutIfActive(
                    command.FixtureId, command.TeamId
                );
                if (!active) {
                    return new HandleResult<PlayerRatingDto> {
                        Error = new LivescoreError("Fixture is no longer active")
                    };
                }

                var userVote = await _userVoteInMemRepository.FindOneForFixtureParticipant(
                    userId, command.FixtureId, command.TeamId, command.ParticipantKey
                );

                float? oldRating = userVote.ChangePlayerRating(command.ParticipantKey, command.Rating);
                
                _unitOfWork.Begin();
                
                _fixtureLivescoreStatusInMemRepository.EnlistAsPartOf(_unitOfWork);
                _fixtureLivescoreStatusInMemRepository.WatchStillActive(command.FixtureId, command.TeamId);

                _userVoteInMemRepository.EnlistAsPartOf(_unitOfWork);
                _userVoteInMemRepository.UpdateOneForFixtureParticipant(userVote, oldRating);

                _playerRatingInMemRepository.EnlistAsPartOf(_unitOfWork);
                _playerRatingInMemRepository.UpdateOneFor(
                    command.FixtureId, command.TeamId, command.ParticipantKey,
                    oldRating, command.Rating
                );

                applied = await _unitOfWork.Commit();
            } while (!applied);

            var updatedPlayerRating = await _playerRatingInMemQueryable.GetRatingFor(
                command.FixtureId, command.TeamId, command.ParticipantKey
            );

            return new HandleResult<PlayerRatingDto> {
                Data = new PlayerRatingDto {
                    ParticipantKey = updatedPlayerRating.ParticipantKey,
                    TotalRating = updatedPlayerRating.TotalRating,
                    TotalVoters = updatedPlayerRating.TotalVoters
                }
            };
        }
    }
}
