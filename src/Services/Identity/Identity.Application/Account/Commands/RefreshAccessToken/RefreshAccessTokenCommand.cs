using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Identity.Application.Account.Common.Dto;
using Identity.Application.Account.Common.Errors;
using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Results;
using Identity.Domain.Aggregates.User;
using Identity.Domain.Base;

namespace Identity.Application.Account.Commands.RefreshAccessToken {
    public class RefreshAccessTokenCommand : IRequest<HandleResult<SecurityCredentialsDto>> {
        public string DeviceId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RefreshAccessTokenCommandHandler : IRequestHandler<
        RefreshAccessTokenCommand, HandleResult<SecurityCredentialsDto>
    > {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISecurityTokenProvider _securityTokenProvider;
        private readonly IPrincipalDataProvider _principalDataProvider;
        private readonly IUserService _userService;

        public RefreshAccessTokenCommandHandler(
            IUnitOfWork unitOfWork,
            ISecurityTokenProvider securityTokenProvider,
            IPrincipalDataProvider principalDataProvider,
            IUserService userService
        ) {
            _unitOfWork = unitOfWork;
            _securityTokenProvider = securityTokenProvider;
            _principalDataProvider = principalDataProvider;
            _userService = userService;
        }

        public async Task<HandleResult<SecurityCredentialsDto>> Handle(
            RefreshAccessTokenCommand command, CancellationToken cancellationToken
        ) {
            var outcome = _securityTokenProvider.CreatePrincipalFromAccessToken(command.AccessToken);
            if (outcome.IsError) {
                return new HandleResult<SecurityCredentialsDto> {
                    Error = outcome.Error
                };
            }

            await _unitOfWork.Begin();
            try {
                _userService.EnlistAsPartOf(_unitOfWork);

                long userId = _principalDataProvider.GetId(outcome.Data);

                var findOutcome = await _userService.FindById(userId, includeRefreshTokensAndClaims: true);
                if (findOutcome.IsError) {
                    return new HandleResult<SecurityCredentialsDto> {
                        Error = findOutcome.Error
                    };
                }

                var user = findOutcome.Data;

                var refreshToken = user.RefreshTokens.FirstOrDefault(
                    t => t.DeviceId == command.DeviceId && t.Value == command.RefreshToken
                );
                if (refreshToken == null) {
                    return new HandleResult<SecurityCredentialsDto> {
                        Error = new AccountError("Invalid refresh token")
                    };
                }

                user.RemoveRefreshToken(refreshToken);

                if (refreshToken.IsValid) {
                    refreshToken = new RefreshToken(
                        deviceId: command.DeviceId,
                        value: _securityTokenProvider.GenerateRefreshToken(),
                        isActive: true,
                        expiresAt: DateTimeOffset.UtcNow.AddDays(30) // @@TODO: Config.
                    );

                    user.AddRefreshToken(refreshToken);
                }

                await _userService.UpdateRefreshTokens(user);

                await _unitOfWork.Commit();
                
                if (!refreshToken.IsValid) {
                    return new HandleResult<SecurityCredentialsDto> {
                        Error = new AccountError("Refresh token has expired")
                    };
                }

                var claims = user.Claims.ToList();

                return new HandleResult<SecurityCredentialsDto> {
                    Data = new SecurityCredentialsDto {
                        AccessToken = _securityTokenProvider.GenerateJwt(user.Id, claims),
                        RefreshToken = refreshToken.Value
                    }
                };
            } catch {
                await _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
