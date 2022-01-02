using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MediatR;

using Identity.Application.Account.Common.Dto;
using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Results;
using Identity.Domain.Aggregates.User;

namespace Identity.Application.Account.Commands.SignIn {
    public class SignInCommand : IRequest<HandleResult<SecurityCredentialsDto>> {
        public string DeviceId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class SignInCommandHandler : IRequestHandler<
        SignInCommand, HandleResult<SecurityCredentialsDto>
    > {
        private readonly ILogger<SignInCommandHandler> _logger;
        private readonly IUserService _userService;
        private readonly ISecurityTokenProvider _securityTokenProvider;

        public SignInCommandHandler(
            ILogger<SignInCommandHandler> logger,
            IUserService userService,
            ISecurityTokenProvider securityTokenProvider
        ) {
            _logger = logger;
            _userService = userService;
            _securityTokenProvider = securityTokenProvider;
        }

        public async Task<HandleResult<SecurityCredentialsDto>> Handle(
            SignInCommand command, CancellationToken cancellationToken
        ) {
            var outcome = await _userService.FindByEmail(
                command.Email, includeRefreshTokensAndClaims: true
            );
            if (outcome.IsError) {
                return new HandleResult<SecurityCredentialsDto> {
                    Error = outcome.Error
                };
            }

            var user = outcome.Data;

            var verifyOutcome = await _userService.VerifyPassword(user, command.Password);
            if (verifyOutcome.IsError) {
                return new HandleResult<SecurityCredentialsDto> {
                    Error = verifyOutcome.Error
                };
            }

            user.RemoveAllRefreshTokensForDevice(command.DeviceId);

            var refreshToken = new RefreshToken(
                deviceId: command.DeviceId,
                value: _securityTokenProvider.GenerateRefreshToken(),
                isActive: true,
                expiresAt: DateTimeOffset.UtcNow.AddDays(60) // @@TODO: Config.
            );

            user.AddRefreshToken(refreshToken);

            await _userService.FinalizeSignIn(user);

            return new HandleResult<SecurityCredentialsDto> {
                Data = new SecurityCredentialsDto {
                    Username = user.Username,
                    AccessToken = _securityTokenProvider.GenerateJwt(user.Id, user.Claims.ToList()),
                    RefreshToken = refreshToken.Value
                }
            };
        }
    }
}
