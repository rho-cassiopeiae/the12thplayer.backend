using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Livescore.Api.Hubs.Clients;
using Livescore.Api.Hubs.Filters;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.PlayerRating.Queries.GetPlayerRatingsForFixture;
using Livescore.Application.Livescore.PlayerRating.Commands.RatePlayer;

namespace Livescore.Api.Hubs {
    public partial class FanzoneHub : Hub<ILivescoreClient> {
        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<HandleResult<FixturePlayerRatingsDto>> GetPlayerRatingsForFixture(
            GetPlayerRatingsForFixtureQuery query
        ) => _mediator.Send(query);

        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<HandleResult<PlayerRatingDto>> RatePlayer(RatePlayerCommand command) => _mediator.Send(command);
    }
}
