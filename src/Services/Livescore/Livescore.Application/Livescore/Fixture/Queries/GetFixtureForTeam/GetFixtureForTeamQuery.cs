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
        private readonly ILivescoreQueryable _livescoreQueryable;

        public GetFixtureForTeamQueryHandler(ILivescoreQueryable livescoreQueryable) {
            _livescoreQueryable = livescoreQueryable;
        }

        public async Task<HandleResult<FixtureFullDto>> Handle(
            GetFixtureForTeamQuery query, CancellationToken cancellationToken
        ) {
            var fixture = await _livescoreQueryable.GetFixtureForTeam(query.FixtureId, query.TeamId);
            
            return new HandleResult<FixtureFullDto> {
                Data = fixture
            };
        }
    }
}
