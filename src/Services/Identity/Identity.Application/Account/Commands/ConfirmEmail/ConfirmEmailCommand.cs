using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MediatR;

using Identity.Application.Account.Common.Errors;
using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Results;
using Identity.Domain.Aggregates.User;

namespace Identity.Application.Account.Commands.ConfirmEmail {
    public class ConfirmEmailCommand : IRequest<HandleResult<SecurityCredentials>> {
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
    }

    public class ConfirmEmailCommandHandler : IRequestHandler<
        ConfirmEmailCommand, HandleResult<SecurityCredentials>
    > {
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;
        private readonly IUserService _userService;
        private readonly ISecurityTokenProvider _securityTokenProvider;

        public ConfirmEmailCommandHandler(
            ILogger<ConfirmEmailCommandHandler> logger,
            IUserService userService,
            ISecurityTokenProvider securityTokenProvider
        ) {
            _logger = logger;
            _userService = userService;
            _securityTokenProvider = securityTokenProvider;
        }

        public async Task<HandleResult<SecurityCredentials>> Handle(
            ConfirmEmailCommand command, CancellationToken cancellationToken
        ) {
            var email = command.Email;

            var outcome = await _userService.FindByEmail(email);
            if (outcome.IsError) {
                return new HandleResult<SecurityCredentials> {
                    Error = outcome.Error
                };
            }

            var user = outcome.Data;

            if (user.IsConfirmed) {
                return new HandleResult<SecurityCredentials> {
                    Error = new AccountError(
                        $"Account {email} has already been confirmed"
                    )
                };
            }

            var success = await _userService.VerifyEmailConfirmationCode(
                user, command.ConfirmationCode
            );
            if (!success) {
                return new HandleResult<SecurityCredentials> {
                    Error = new AccountError(
                        $"Account {email}: invalid confirmation code"
                    )
                };
            }

            user.SetConfirmed();
            user.AddClaim("__Username", user.Username);

            var refreshToken = new RefreshToken(
                value: _securityTokenProvider.GenerateRefreshToken(),
                isActive: true,
                expiresAt: DateTimeOffset.UtcNow.AddDays(30)
            );

            user.AddRefreshToken(refreshToken);

            var finOutcome = await _userService.FinalizeAccountCreation(user);
            if (finOutcome.IsError) {
                return new HandleResult<SecurityCredentials> {
                    Error = finOutcome.Error
                };
            }

            return new HandleResult<SecurityCredentials> {
                Data = new SecurityCredentials {
                    AccessToken = _securityTokenProvider.GenerateJwt(
                        user.Id, user.Claims.ToList()
                    ),
                    RefreshToken = refreshToken.Value
                }
            };
        }
    }
}
