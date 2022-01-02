using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.FixtureLivescoreStatus;
using Livescore.Domain.Aggregates.PlayerRating;
using Livescore.Domain.Aggregates.UserVote;
using Livescore.Domain.Base;
using Livescore.Domain.Aggregates.Discussion;
using Livescore.Domain.Aggregates.VideoReaction;

namespace Livescore.Application.Livescore.Worker.Commands.DeactivateFixture {
    public class DeactivateFixtureCommand : IRequest<VoidResult> {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
    }

    public class DeactivateFixtureCommandHandler : IRequestHandler<DeactivateFixtureCommand, VoidResult> {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInMemUnitOfWork _inMemUnitOfWork;

        private readonly IFixtureLivescoreStatusInMemRepository _fixtureLivescoreStatusInMemRepository;
        private readonly IPlayerRatingInMemRepository _playerRatingInMemRepository;
        private readonly IVideoReactionInMemRepository _videoReactionInMemRepository;
        private readonly IUserVoteInMemRepository _userVoteInMemRepository;
        private readonly IDiscussionInMemRepository _discussionInMemRepository;

        private readonly IPlayerRatingRepository _playerRatingRepository;
        private readonly IUserVoteRepository _userVoteRepository;

        public DeactivateFixtureCommandHandler(
            IUnitOfWork unitOfWork,
            IInMemUnitOfWork inMemUnitOfWork,
            IFixtureLivescoreStatusInMemRepository fixtureLivescoreStatusInMemRepository,
            IPlayerRatingInMemRepository playerRatingInMemRepository,
            IVideoReactionInMemRepository videoReactionInMemRepository,
            IUserVoteInMemRepository userVoteInMemRepository,
            IDiscussionInMemRepository discussionInMemRepository,
            IPlayerRatingRepository playerRatingRepository,
            IUserVoteRepository userVoteRepository
        ) {
            _unitOfWork = unitOfWork;
            _inMemUnitOfWork = inMemUnitOfWork;
            _fixtureLivescoreStatusInMemRepository = fixtureLivescoreStatusInMemRepository;
            _playerRatingInMemRepository = playerRatingInMemRepository;
            _videoReactionInMemRepository = videoReactionInMemRepository;
            _userVoteInMemRepository = userVoteInMemRepository;
            _discussionInMemRepository = discussionInMemRepository;
            _playerRatingRepository = playerRatingRepository;
            _userVoteRepository = userVoteRepository;
        }

        public async Task<VoidResult> Handle(
            DeactivateFixtureCommand command, CancellationToken cancellationToken
        ) {
            var fixtureStatus = new FixtureLivescoreStatus(
                fixtureId: command.FixtureId,
                teamId: command.TeamId,
                active: false,
                ongoing: null
            );

            _fixtureLivescoreStatusInMemRepository.SetOrUpdate(fixtureStatus);
            await _fixtureLivescoreStatusInMemRepository.SaveChanges();

            var playerRatings = await _playerRatingInMemRepository.FindAllFor(command.FixtureId, command.TeamId);

            var fixtureParticipantKeys = playerRatings.Select(pr => pr.ParticipantKey).ToList();

            var userVotes = await _userVoteInMemRepository.FindAllFor(
                command.FixtureId, command.TeamId,
                fixtureParticipantKeys
            );

            var discussions = await _discussionInMemRepository.FindAllFor(command.FixtureId, command.TeamId);

            if (playerRatings.Any()) {
                await _unitOfWork.Begin();

                _playerRatingRepository.EnlistAsPartOf(_unitOfWork);
                await _playerRatingRepository.Create(playerRatings);
                
                if (userVotes.Any()) {
                    await _userVoteRepository.Create(userVotes);
                }

                await _unitOfWork.Commit();
            }

            _inMemUnitOfWork.Begin();

            _playerRatingInMemRepository.EnlistAsPartOf(_inMemUnitOfWork);
            _playerRatingInMemRepository.DeleteAllFor(command.FixtureId, command.TeamId);

            _videoReactionInMemRepository.EnlistAsPartOf(_inMemUnitOfWork);
            _videoReactionInMemRepository.DeleteAllFor(command.FixtureId, command.TeamId);

            _userVoteInMemRepository.EnlistAsPartOf(_inMemUnitOfWork);
            _userVoteInMemRepository.DeleteAllFor(command.FixtureId, command.TeamId, fixtureParticipantKeys);

            _discussionInMemRepository.EnlistAsPartOf(_inMemUnitOfWork);
            _discussionInMemRepository.DeleteAllFor(
                command.FixtureId, command.TeamId, discussions.Select(d => d.Id).ToList()
            );

            await _inMemUnitOfWork.Commit();

            return VoidResult.Instance;
        }
    }
}
