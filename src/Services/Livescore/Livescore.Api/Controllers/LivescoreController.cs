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
using Livescore.Application.Team.Queries.GetTeamSquad.Dto;
using Livescore.Application.Team.Queries.GetTeamSquad;
using Livescore.Application.Team.Queries.GetPlayerRatingsForParticipant;

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

        // /livescore/teams/{teamId}/squad
        [HttpGet("squad")]
        public Task<HandleResult<TeamSquadDto>> GetTeamSquad(GetTeamSquadQuery query) => _mediator.Send(query);

        // /livescore/teams/{teamId}/players/{playerId}/ratings
        [HttpGet("players/{playerId}/ratings")]
        public Task<HandleResult<IEnumerable<FixturePlayerRatingDto>>> GetRatingsForPlayer(long teamId, long playerId) =>
            _mediator.Send(new GetPlayerRatingsForParticipantQuery {
                TeamId = teamId,
                ParticipantKeys = new[] { $"p:{playerId}", $"s:{playerId}" }
            });

        // /livescore/teams/{teamId}/managers/{managerId}/ratings
        [HttpGet("managers/{managerId}/ratings")]
        public Task<HandleResult<IEnumerable<FixturePlayerRatingDto>>> GetRatingsForManager(long teamId, long managerId) =>
            _mediator.Send(new GetPlayerRatingsForParticipantQuery {
                TeamId = teamId,
                ParticipantKeys = new[] { $"m:{managerId}" }
            });
    }
}
