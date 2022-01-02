using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Livescore.Api.Hubs.Clients;
using Livescore.Api.Hubs.Filters;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.VideoReaction.Queries.GetVideoReactionsForFixture;
using Livescore.Application.Livescore.VideoReaction.Commands.VoteForVideoReaction;

namespace Livescore.Api.Hubs {
    public partial class FanzoneHub : Hub<ILivescoreClient> {
        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<HandleResult<FixtureVideoReactionsDto>> GetVideoReactionsForFixture(
            GetVideoReactionsForFixtureQuery query
        ) => _mediator.Send(query);

        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<HandleResult<VideoReactionRatingDto>> VoteForVideoReaction(
            VoteForVideoReactionCommand command
        ) => _mediator.Send(command);
    }
}
