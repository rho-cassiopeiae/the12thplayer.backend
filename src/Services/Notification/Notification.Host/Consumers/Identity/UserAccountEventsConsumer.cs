using System.Threading.Tasks;

using MassTransit;
using MediatR;

using MessageBus.Contracts.Events.Identity;

using Notification.Application.Identity.Commands.SendAccountConfirmationEmail;

namespace Notification.Host.Consumers.Identity {
    public class UserAccountEventsConsumer : IConsumer<UserAccountCreated> {
        private readonly ISender _mediator;

        public UserAccountEventsConsumer(ISender mediator) {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UserAccountCreated> context) {
            var command = new SendAccountConfirmationEmailCommand {
                Email = context.Message.Email,
                Username = context.Message.Username,
                ConfirmationCode = context.Message.ConfirmationCode
            };

            await _mediator.Send(command);
        }
    }
}
