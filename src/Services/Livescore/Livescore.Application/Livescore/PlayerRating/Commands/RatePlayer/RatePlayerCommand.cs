using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.PlayerRating;

namespace Livescore.Application.Livescore.PlayerRating.Commands.RatePlayer {
    public class RatePlayerCommand : IRequest<HandleResult<PlayerRatingDto>> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public string ParticipantKey { get; set; }
        public float Rating { get; set; }
    }

    public class RatePlayerCommandHandler : IRequestHandler<
        RatePlayerCommand, HandleResult<PlayerRatingDto>
    > {
        private readonly IPlayerRatingInMemRepository _playerRatingInMemRepository;
        private readonly IPlayerRatingInMemQueryable _playerRatingInMemQueryable;

        public RatePlayerCommandHandler(
            IPlayerRatingInMemRepository playerRatingInMemRepository,
            IPlayerRatingInMemQueryable playerRatingInMemQueryable
        ) {
            _playerRatingInMemRepository = playerRatingInMemRepository;
            _playerRatingInMemQueryable = playerRatingInMemQueryable;
        }

        public async Task<HandleResult<PlayerRatingDto>> Handle(
            RatePlayerCommand command, CancellationToken cancellationToken
        ) {
            bool applied;
            do {
                var playerRating = await _playerRatingInMemRepository.FindUserVoteForPlayer(
                    command.FixtureId, command.TeamId, 1, command.ParticipantKey
                );

                var userVote = playerRating.UserVotes.Single();
                var newUserVote = new UserVote(
                    userId: userVote.UserId,
                    currentRating: userVote.CurrentRating,
                    newRating: (int) command.Rating
                );

                playerRating.RemoveUserVote(userVote);
                playerRating.AddUserVote(newUserVote);

                _playerRatingInMemRepository.Update(playerRating);

                applied = await _playerRatingInMemRepository.SaveChanges();
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
