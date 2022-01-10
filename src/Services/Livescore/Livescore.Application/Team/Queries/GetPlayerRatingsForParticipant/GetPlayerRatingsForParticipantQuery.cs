using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;

namespace Livescore.Application.Team.Queries.GetPlayerRatingsForParticipant {
    public class GetPlayerRatingsForParticipantQuery : IRequest<HandleResult<IEnumerable<FixturePlayerRatingDto>>> {
        public long TeamId { get; init; }
        public string[] ParticipantKeys { get; init; }
    }

    public class GetPlayerRatingsForParticipantQueryHandler : IRequestHandler<
        GetPlayerRatingsForParticipantQuery, HandleResult<IEnumerable<FixturePlayerRatingDto>>
    > {
        private readonly IPlayerRatingQueryable _playerRatingQueryable;

        public GetPlayerRatingsForParticipantQueryHandler(IPlayerRatingQueryable playerRatingQueryable) {
            _playerRatingQueryable = playerRatingQueryable;
        }

        public async Task<HandleResult<IEnumerable<FixturePlayerRatingDto>>> Handle(
            GetPlayerRatingsForParticipantQuery query, CancellationToken cancellationToken
        ) {
            return new HandleResult<IEnumerable<FixturePlayerRatingDto>> {
                Data = await _playerRatingQueryable.GetAllFor(query.TeamId, query.ParticipantKeys)
            };
        }
    }
}
