using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.Common.Errors;
using Livescore.Domain.Aggregates.FixtureLivescoreStatus;
using Livescore.Domain.Aggregates.UserVote;
using Livescore.Domain.Aggregates.VideoReaction;
using Livescore.Domain.Base;
using Livescore.Application.Common.Attributes;

namespace Livescore.Application.Livescore.VideoReaction.Commands.VoteForVideoReaction {
    [RequireAuthorization]
    public class VoteForVideoReactionCommand : IRequest<HandleResult<VideoReactionRatingDto>> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public long AuthorId { get; set; }
        public short? UserVote { get; set; }
    }

    public class VoteForVideoReactionCommandHandler : IRequestHandler<
        VoteForVideoReactionCommand, HandleResult<VideoReactionRatingDto>
    > {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;
        private readonly IInMemUnitOfWork _unitOfWork;
        private readonly IFixtureLivescoreStatusInMemRepository _fixtureLivescoreStatusInMemRepository;
        private readonly IUserVoteInMemRepository _userVoteInMemRepository;
        private readonly IVideoReactionInMemRepository _videoReactionInMemRepository;
        private readonly IVideoReactionInMemQueryable _videoReactionInMemQueryable;

        public VoteForVideoReactionCommandHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IInMemUnitOfWork unitOfWork,
            IFixtureLivescoreStatusInMemRepository fixtureLivescoreStatusInMemRepository,
            IUserVoteInMemRepository userVoteInMemRepository,
            IVideoReactionInMemRepository videoReactionInMemRepository,
            IVideoReactionInMemQueryable videoReactionInMemQueryable
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _unitOfWork = unitOfWork;
            _fixtureLivescoreStatusInMemRepository = fixtureLivescoreStatusInMemRepository;
            _userVoteInMemRepository = userVoteInMemRepository;
            _videoReactionInMemRepository = videoReactionInMemRepository;
            _videoReactionInMemQueryable = videoReactionInMemQueryable;
        }

        public async Task<HandleResult<VideoReactionRatingDto>> Handle(
            VoteForVideoReactionCommand command, CancellationToken cancellationToken
        ) {
            long userId = _principalDataProvider.GetId(_authenticationContext.User);

            bool applied;
            UserVote userVote;
            do {
                bool active = await _fixtureLivescoreStatusInMemRepository.FindOutIfActive(
                    command.FixtureId, command.TeamId
                );
                if (!active) {
                    return new HandleResult<VideoReactionRatingDto> {
                        Error = new LivescoreError("Fixture is no longer active")
                    };
                }

                userVote = await _userVoteInMemRepository.FindOneForVideoReaction(
                    userId, command.FixtureId, command.TeamId, command.AuthorId
                );

                (short? oldVote, int incrementRatingBy) = userVote.ChangeVideoReactionVote(command.AuthorId, command.UserVote);

                _unitOfWork.Begin();

                _fixtureLivescoreStatusInMemRepository.EnlistAsPartOf(_unitOfWork);
                _fixtureLivescoreStatusInMemRepository.WatchStillActive(command.FixtureId, command.TeamId);

                _userVoteInMemRepository.EnlistAsPartOf(_unitOfWork);
                _userVoteInMemRepository.UpdateOneForVideoReaction(userVote, oldVote);

                _videoReactionInMemRepository.EnlistAsPartOf(_unitOfWork);
                _videoReactionInMemRepository.UpdateRatingFor(
                    command.FixtureId, command.TeamId, command.AuthorId, incrementRatingBy
                );

                applied = await _unitOfWork.Commit();
            } while (!applied);

            int updatedRating = await _videoReactionInMemQueryable.GetRatingFor(
                command.FixtureId, command.TeamId, command.AuthorId
            );

            return new HandleResult<VideoReactionRatingDto> {
                Data = new VideoReactionRatingDto {
                    Rating = updatedRating
                }
            };
        }
    }
}
