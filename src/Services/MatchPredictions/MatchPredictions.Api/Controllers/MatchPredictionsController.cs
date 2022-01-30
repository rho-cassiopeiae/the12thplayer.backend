using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using MatchPredictions.Application.Common.Results;
using MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam;
using MatchPredictions.Application.Playtime.Commands.SubmitMatchPredictions;

namespace MatchPredictions.Api.Controllers {
    [Route("match-predictions")]
    public class MatchPredictionsController {
        private readonly ISender _mediator;

        public MatchPredictionsController(ISender mediator) {
            _mediator = mediator;
        }

        // match-predictions/teams/{teamId}
        [HttpGet("teams/{teamId}")]
        public Task<HandleResult<IEnumerable<ActiveSeasonRoundWithFixturesDto>>> GetActiveFixturesForTeam(
            GetActiveFixturesForTeamQuery query
        ) => _mediator.Send(query);

        // match-predictions/seasons/{seasonId}/rounds/{roundId}
        [HttpPost("seasons/{seasonId}/rounds/{roundId}")]
        public Task<HandleResult<MatchPredictionsSubmissionDto>> SubmitMatchPredictions(
            long seasonId, long roundId,
            [FromBody] Dictionary<string, string> predictions
        ) => _mediator.Send(new SubmitMatchPredictionsCommand {
            SeasonId = seasonId,
            RoundId = roundId,
            FixtureIdToScore = predictions
        });
    }
}
