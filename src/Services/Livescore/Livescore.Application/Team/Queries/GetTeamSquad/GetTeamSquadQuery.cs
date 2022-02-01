using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;

namespace Livescore.Application.Team.Queries.GetTeamSquad {
    public class GetTeamSquadQuery : IRequest<HandleResult<TeamSquadDto>> {
        public long TeamId { get; set; }
    }

    public class GetTeamSquadQueryHandler : IRequestHandler<GetTeamSquadQuery, HandleResult<TeamSquadDto>> {
        private readonly IPlayerQueryable _playerQueryable;
        private readonly IManagerQueryable _managerQueryable;

        public GetTeamSquadQueryHandler(
            IPlayerQueryable playerQueryable,
            IManagerQueryable managerQueryable
        ) {
            _playerQueryable = playerQueryable;
            _managerQueryable = managerQueryable;
        }

        public async Task<HandleResult<TeamSquadDto>> Handle(
            GetTeamSquadQuery query, CancellationToken cancellationToken
        ) {
            var players = await _playerQueryable.GetPlayersWithCountryFrom(query.TeamId);
            var manager = await _managerQueryable.GetManagerWithCountryFrom(query.TeamId);

            return new HandleResult<TeamSquadDto> {
                Data = new TeamSquadDto {
                    Manager = manager,
                    Players = players
                }
            };
        }
    }
}
