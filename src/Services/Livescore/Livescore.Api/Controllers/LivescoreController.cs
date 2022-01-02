using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.Fixture.Common.Dto;
using Livescore.Application.Livescore.Fixture.Queries.GetFixturesForTeamInBetween;
using Livescore.Application.Livescore.Fixture.Queries.GetFixtureForTeam;
using Livescore.Api.Controllers.Filters;
using Livescore.Application.Livescore.VideoReaction.Commands.PostVideoReaction;

namespace Livescore.Api.Controllers {
    [Route("[controller]/teams/{teamId}")]
    public class LivescoreController : ControllerBase {
        private readonly ISender _mediator;

        public LivescoreController(ISender mediator) {
            _mediator = mediator;
        }

        // /livescore/teams/{teamId}/fixtures?startTime={startTime}&endTime={endTime}
        [HttpGet("fixtures")]
        public Task<HandleResult<IEnumerable<FixtureSummaryDto>>> GetFixturesForTeamInBetween(
            GetFixturesForTeamInBetweenQuery query
        ) => _mediator.Send(query);

        // /livescore/teams/{teamId}/fixtures/{fixtureId}
        [HttpGet("fixtures/{fixtureId}")]
        public Task<HandleResult<FixtureFullDto>> GetFixtureForTeam(GetFixtureForTeamQuery query) =>
            _mediator.Send(query);

        // /livescore/teams/{teamId}/fixtures/{fixtureId}/video-reactions
        [DisableFormValueModelBinding]
        [RequestSizeLimit(50 * 1024 * 1024)] // @@TODO: Config.
        [HttpPost("fixtures/{fixtureId}/video-reactions")]
        public Task<HandleResult<string>> PostVideoReaction(long fixtureId, long teamId) =>
            _mediator.Send(new PostVideoReactionCommand {
                FixtureId = fixtureId,
                TeamId = teamId,
                Request = Request
            });
    }
}
