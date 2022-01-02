using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Livescore.Api.Hubs.Clients;
using Livescore.Api.Hubs.Filters;
using Livescore.Application.Common.Dto;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.Discussion.Queries.GetDiscussionsForFixture;
using Livescore.Application.Livescore.Discussion.Commands.SubscribeToDiscussion;
using Livescore.Application.Livescore.Discussion.Commands.UnsubscribeFromDiscussion;
using Livescore.Application.Livescore.Discussion.Queries.GetMoreDiscussionEntries;
using Livescore.Application.Livescore.Discussion.Commands.PostDiscussionEntry;

namespace Livescore.Api.Hubs {
    public partial class FanzoneHub : Hub<ILivescoreClient> {
        public Task<HandleResult<IEnumerable<DiscussionDto>>> GetDiscussionsForFixture(
            GetDiscussionsForFixtureQuery query
        ) => _mediator.Send(query);

        [AddConnectionIdProviderToMethodInvocationScope]
        public Task<HandleResult<FixtureDiscussionUpdateDto>> SubscribeToDiscussion(
            SubscribeToDiscussionCommand command
        ) => _mediator.Send(command);

        [AddConnectionIdProviderToMethodInvocationScope]
        public Task<VoidResult> UnsubscribeFromDiscussion(UnsubscribeFromDiscussionCommand command) =>
            _mediator.Send(command);

        public Task<HandleResult<IEnumerable<DiscussionEntryDto>>> GetMoreDiscussionEntries(
            GetMoreDiscussionEntriesQuery query
        ) => _mediator.Send(query);

        [CopyAuthenticationContextToMethodInvocationScope]
        public Task<VoidResult> PostDiscussionEntry(PostDiscussionEntryCommand command) => _mediator.Send(command);
    }
}
