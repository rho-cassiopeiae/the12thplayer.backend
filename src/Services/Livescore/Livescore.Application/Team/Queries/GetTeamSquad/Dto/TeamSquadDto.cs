using System.Collections.Generic;

namespace Livescore.Application.Team.Queries.GetTeamSquad.Dto {
    public class TeamSquadDto {
        public ManagerDto Manager { get; init; }
        public IEnumerable<PlayerDto> Players { get; init; }
    }
}
