using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.Fixture.Common.Dto;
using Livescore.Application.Livescore.Fixture.Queries.GetFixturesForTeamInBetween;
using Livescore.Application.Livescore.Fixture.Queries.GetFixtureForTeam;

namespace Livescore.Api.Controllers {
    [Route("fixtures")]
    public class FixtureController {
        private readonly ISender _mediator;

        public FixtureController(ISender mediator) {
            _mediator = mediator;
        }

        // /fixtures?teamId={teamId}&startTime={startTime}&endTime={endTime}
        [HttpGet]
        public Task<HandleResult<IEnumerable<FixtureSummaryDto>>> GetFixturesForTeamInBetween(
            GetFixturesForTeamInBetweenQuery query
        ) => _mediator.Send(query);

        // /fixtures/{fixtureId}?teamId={teamId}
        [HttpGet("{fixtureId}")]
        public Task<HandleResult<FixtureFullDto>> GetFixtureForTeam(GetFixtureForTeamQuery query) =>
            _mediator.Send(query);
    }
}
