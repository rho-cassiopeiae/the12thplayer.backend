using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using MatchPredictions.Application.Common.Results;
using MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam;

namespace MatchPredictions.Api.Controllers {
    [Route("match-predictions/teams/{teamId}")]
    public class MatchPredictionsController {
        private readonly ISender _mediator;

        public MatchPredictionsController(ISender mediator) {
            _mediator = mediator;
        }

        // match-predictions/teams/{teamId}
        [HttpGet]
        public Task<HandleResult<IEnumerable<ActiveSeasonRoundWithFixturesDto>>> GetActiveFixturesForTeam(
            GetActiveFixturesForTeamQuery query
        ) => _mediator.Send(query);
    }
}
