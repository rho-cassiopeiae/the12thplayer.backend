using System.Threading.Tasks;

using MassTransit;
using MediatR;

using MessageBus.Contracts.Events.Identity;

using Feed.Application.Author.Commands.CreateAuthor;

namespace Feed.Api.Consumers.Identity {
    public class UserAccountEventsConsumer : IConsumer<UserAccountConfirmed> {
        private readonly ISender _mediator;

        public UserAccountEventsConsumer(ISender mediator) {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UserAccountConfirmed> context) {
            var command = new CreateAuthorCommand {
                UserId = context.Message.UserId,
                Email = context.Message.Email,
                Username = context.Message.Username
            };

            await _mediator.Send(command);
        }
    }
}
