using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Application.Common.Dto;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Application.Livescore.Worker.Queries.GetTeamPlayers {
    public class GetTeamPlayersQuery : IRequest<HandleResult<IEnumerable<PlayerDto>>> {
        public long TeamId { get; init; }
    }

    public class GetTeamPlayersQueryHandler : IRequestHandler<
        GetTeamPlayersQuery, HandleResult<IEnumerable<PlayerDto>>
    > {
        private readonly ILivescoreQueryable _livescoreQueryable;

        public GetTeamPlayersQueryHandler(ILivescoreQueryable livescoreQueryable) {
            _livescoreQueryable = livescoreQueryable;
        }

        public async Task<HandleResult<IEnumerable<PlayerDto>>> Handle(
            GetTeamPlayersQuery query, CancellationToken cancellationToken
        ) {
            var players = await _livescoreQueryable.GetPlayersFrom(query.TeamId);

            return new HandleResult<IEnumerable<PlayerDto>> {
                Data = players.Select(p => new PlayerDto {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    ImageUrl = p.ImageUrl
                })
            };
        }
    }
}
