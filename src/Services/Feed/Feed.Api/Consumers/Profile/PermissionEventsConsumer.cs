using System.Linq;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using MessageBus.Contracts.Events.Profile;

using Feed.Application.Author.Commands.AddPermissions;
using Feed.Application.Author.Common.Dto;

namespace Feed.Api.Consumers.Profile {
    public class PermissionEventsConsumer : IConsumer<ProfilePermissionsGranted> {
        private readonly ISender _mediator;

        public PermissionEventsConsumer(ISender mediator) {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ProfilePermissionsGranted> context) {
            var command = new AddPermissionsCommand {
                UserId = context.Message.UserId,
                Permissions = context.Message.Permissions.Select(
                    p => new AuthorPermissionDto {
                        Scope = p.Scope,
                        Flags = p.Flags
                    }
                )
            };

            await _mediator.Send(command);
        }
    }
}
