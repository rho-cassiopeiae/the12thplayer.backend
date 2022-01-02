using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MediatR;

using Notification.Application.Common.Results;

namespace Notification.Application.Identity.Commands.SendAccountConfirmationEmail {
    public class SendAccountConfirmationEmailCommand : IRequest<VoidResult> {
        public string Email { get; init; }
        public string Username { get; init; }
        public string ConfirmationCode { get; init; }
    }

    public class SendAccountConfirmationEmailCommandHandler : IRequestHandler<
        SendAccountConfirmationEmailCommand, VoidResult
    > {
        private readonly ILogger<SendAccountConfirmationEmailCommandHandler> _logger;

        public SendAccountConfirmationEmailCommandHandler(
            ILogger<SendAccountConfirmationEmailCommandHandler> logger
        ) {
            _logger = logger;
        }

        public async Task<VoidResult> Handle(
            SendAccountConfirmationEmailCommand command, CancellationToken cancellationToken
        ) {
            _logger.LogTrace(
                "Hello, {Username}! Welcome to The12thPlayer community!\nYour confirmation code is {Code}\n\nThanks for joining us.",
                command.Username, command.ConfirmationCode
            );

            return VoidResult.Instance;
        }
    }
}
