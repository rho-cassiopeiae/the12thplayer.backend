using System;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using MessageBus.Contracts.Requests.Admin;
using MessageBus.Contracts.Requests.Identity;
using MessageBus.Contracts.Responses.Profile;

using Profile.Application.Profile.Common.Dto;
using Profile.Application.Profile.Queries.CheckHasPermissions;
using Profile.Application.Profile.Queries.GetPermissions;

namespace Profile.Api.Consumers.Identity {
    public class PermissionRequestsConsumer :
        IConsumer<CollectProfilePermissions>,
        IConsumer<CheckProfileHasPermissions> {
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

        public async Task Consume(
            ConsumeContext<CheckProfileHasPermissions> context
        ) {
            var query = new CheckHasPermissionsQuery {
                UserId = context.Message.UserId,
                Permissions = context.Message.Permissions.Select(p =>
                    new ProfilePermission {
                        Scope = p.Scope,
                        Flags = p.Flags
                    }
                )
            };

            var result = await _mediator.Send(query);

            await context.RespondAsync(new CheckProfileHasPermissionsSuccess {
                CorrelationId = Guid.NewGuid(),
                HasRequiredPermissions = result.Data
            });
        }
    }
}
