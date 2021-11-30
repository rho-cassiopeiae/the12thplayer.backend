using System;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using MessageBus.Contracts.Requests.Identity;
using MessageBus.Contracts.Responses.Profile;

using Profile.Application.Profile.Queries.GetPermissions;

namespace Profile.Api.Consumers.Identity {
    public class PermissionRequestsConsumer : IConsumer<CollectProfilePermissions> {
        private readonly ISender _mediator;

        public PermissionRequestsConsumer(ISender mediator) {
            _mediator = mediator;
        }

        public async Task Consume(
            ConsumeContext<CollectProfilePermissions> context
        ) {
            var query = new GetPermissionsQuery {
                UserId = context.Message.UserId
            };

            var result = await _mediator.Send(query);

            await context.RespondAsync(new CollectProfilePermissionsSuccess {
                CorrelationId = Guid.NewGuid(),
                Permissions = result.Data.Select(
                    p => new CollectProfilePermissionsSuccess.ProfilePermission {
                        Scope = (int) p.Scope,
                        Flags = p.Flags
                    }
                )
            });
        }
    }
}
