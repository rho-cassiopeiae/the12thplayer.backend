using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using MediatR;

using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Results;
using Identity.Application.Account.Common.Errors;

namespace Identity.Application.Account.Commands.LogInAsAdmin {
    public class LogInAsAdminCommand : IRequest<HandleResult<string>> {
        public string Email { get; init; }
        public string Password { get; init; }
    }

    public class LogInAsAdminCommandHandler : IRequestHandler<
        LogInAsAdminCommand, HandleResult<string>
    > {
        private readonly IUserService _userService;
        private readonly IProfileSvcQueryable _profileSvcQueryable;
        private readonly ISecurityTokenProvider _securityTokenProvider;

        public LogInAsAdminCommandHandler(
            IUserService userService,
            IProfileSvcQueryable profileSvcQueryable,
            ISecurityTokenProvider securityTokenProvider
        ) {
            _userService = userService;
            _profileSvcQueryable = profileSvcQueryable;
            _securityTokenProvider = securityTokenProvider;
        }

        public async Task<HandleResult<string>> Handle(
            LogInAsAdminCommand command, CancellationToken cancellationToken
        ) {
            var outcome = await _userService.FindByEmail(command.Email);
            if (outcome.IsError) {
                return new HandleResult<string> {
                    Error = outcome.Error
                };
            }

            var user = outcome.Data;

            var verifyOutcome = await _userService.VerifyPassword(
                user, command.Password
            );
            if (verifyOutcome.IsError) {
                return new HandleResult<string> {
                    Error = verifyOutcome.Error
                };
            }

            var permissions = await _profileSvcQueryable.GetPermissionsFor(user.Id);

            if (
                !permissions.Any(p =>
                    p.Scope == PermissionScope.AdminPanel &&
                    ((AdminPanelPermissions) p.Flags & AdminPanelPermissions.LogIn) > 0
                )
            ) {
                return new HandleResult<string> {
                    Error = new AccountError("Unauthorized")
                };
            }

            var claims = new List<Claim> { new("__Admin", "1") };

            return new HandleResult<string> {
                Data = _securityTokenProvider.GenerateJwt(user.Id, claims)
            };
        }
    }
}
