using System;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using MessageBus.Contracts.Requests.Admin;
using MessageBus.Contracts.Requests.Identity;
using MessageBus.Contracts.Responses.Profile;
using ProfilePermissionDtoMsg = MessageBus.Contracts.Common.Dto.ProfilePermissionDto;

using Profile.Application.Profile.Common.Dto;
using Profile.Application.Profile.Queries.CheckHasPermissions;
using Profile.Application.Profile.Queries.GetPermissions;
using Profile.Application.Profile.Commands.GrantPermissions;

namespace Profile.Api.Consumers.Identity {
    public class PermissionRequestsConsumer :
        IConsumer<GetProfilePermissions>,
        IConsumer<CheckProfileHasPermissions>,
        IConsumer<GrantPermissions> {
        private readonly ISender _mediator;

        public PermissionRequestsConsumer(ISender mediator) {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<GetProfilePermissions> context) {
            var query = new GetPermissionsQuery {
                UserId = context.Message.UserId
            };

            var result = await _mediator.Send(query);

            await context.RespondAsync(new GetProfilePermissionsSuccess {
                CorrelationId = Guid.NewGuid(),
                Permissions = result.Data.Select(
                    p => new ProfilePermissionDtoMsg {
                        Scope = (int) p.Scope,
                        Flags = p.Flags
                    }
                )
            });
        }

        public async Task Consume(ConsumeContext<CheckProfileHasPermissions> context) {
            var query = new CheckHasPermissionsQuery {
                UserId = context.Message.UserId,
                Permissions = context.Message.Permissions.Select(p =>
                    new ProfilePermissionDto {
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

        public async Task Consume(ConsumeContext<GrantPermissions> context) {
            var command = new GrantPermissionsCommand {
                UserId = context.Message.UserId,
                Permissions = context.Message.Permissions.Select(p =>
                    new ProfilePermissionDto {
                        Scope = p.Scope,
                        Flags = p.Flags
                    }
                )
            };

            await _mediator.Send(command);

            await context.RespondAsync(new GrantPermissionsSuccess {
                CorrelationId = Guid.NewGuid()
            });
        }
    }
}
