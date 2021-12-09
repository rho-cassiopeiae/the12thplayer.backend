using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.Fixture.Common.Dto;

namespace Livescore.Application.Livescore.Fixture.Queries.GetFixtureForTeam {
    public class GetFixtureForTeamQuery : IRequest<HandleResult<FixtureFullDto>> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
    }

    public class GetFixtureForTeamQueryHandler : IRequestHandler<
        GetFixtureForTeamQuery, HandleResult<FixtureFullDto>
    > {
        private readonly IFixtureQueryable _fixtureQueryable;

        public GetFixtureForTeamQueryHandler(IFixtureQueryable fixtureQueryable) {
            _fixtureQueryable = fixtureQueryable;
        }

        public async Task<HandleResult<FixtureFullDto>> Handle(
            GetFixtureForTeamQuery query, CancellationToken cancellationToken
        ) {
            var fixture = await _fixtureQueryable.GetFixtureForTeam(query.FixtureId, query.TeamId);
            
            return new HandleResult<FixtureFullDto> {
                Data = fixture
            };
        }
    }
}
