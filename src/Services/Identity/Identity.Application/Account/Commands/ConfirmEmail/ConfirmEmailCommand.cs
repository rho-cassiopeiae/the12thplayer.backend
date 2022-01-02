using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MediatR;

using Identity.Application.Account.Common.Errors;
using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Results;
using Identity.Domain.Base;

namespace Identity.Application.Account.Commands.ConfirmEmail {
    public class ConfirmEmailCommand : IRequest<VoidResult> {
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
    }

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, VoidResult> {
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public ConfirmEmailCommandHandler(
            ILogger<ConfirmEmailCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IUserService userService
        ) {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<VoidResult> Handle(
            ConfirmEmailCommand command, CancellationToken cancellationToken
        ) {
            await _unitOfWork.Setup();
            _userService.EnlistConnectionFrom(_unitOfWork);

            var outcome = await _userService.FindByEmail(command.Email);
            if (outcome.IsError) {
                return new VoidResult {
                    Error = outcome.Error
                };
            }

            var user = outcome.Data;

            if (user.IsConfirmed) {
                return new VoidResult {
                    Error = new AccountError($"Account {command.Email} is already confirmed")
                };
            }

            bool success = await _userService.VerifyEmailConfirmationCode(
                user, command.ConfirmationCode
            );
            if (!success) {
                return new VoidResult {
                    Error = new AccountError($"Account {command.Email}: invalid confirmation code")
                };
            }

            user.SetConfirmed();
            user.AddClaim("__Username", user.Username);

            await _unitOfWork.Begin();
            try {
                _userService.EnlistTransactionFrom(_unitOfWork);

                var finOutcome = await _userService.FinalizeAccountCreation(user);
                if (finOutcome.IsError) {
                    return new VoidResult {
                        Error = finOutcome.Error
                    };
                }

                await _userService.DispatchDomainEvents(cancellationToken);

                await _unitOfWork.Commit();

                return VoidResult.Instance;
            } catch {
                await _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
