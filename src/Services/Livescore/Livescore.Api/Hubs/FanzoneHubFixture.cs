using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using MediatR;

using Livescore.Api.Hubs.Clients;
using Livescore.Api.Hubs.Filters;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.Fixture.Commands.SubscribeToFixture;
using Livescore.Application.Livescore.Fixture.Commands.UnsubscribeFromFixture;

namespace Livescore.Api.Hubs {
    public partial class FanzoneHub : Hub<ILivescoreClient> {
        private readonly ISender _mediator;

        public FanzoneHub(ISender mediator) {
            _mediator = mediator;
        }

        [AddConnectionIdProviderToMethodInvocationScope]
        public Task<VoidResult> SubscribeToFixture(SubscribeToFixtureCommand command) => _mediator.Send(command);

        [AddConnectionIdProviderToMethodInvocationScope]
        public Task<VoidResult> UnsubscribeFromFixture(UnsubscribeFromFixtureCommand command) => _mediator.Send(command);
    }
}
