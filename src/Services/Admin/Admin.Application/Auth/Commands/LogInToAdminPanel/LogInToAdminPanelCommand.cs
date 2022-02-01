using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Admin.Application.Common.Interfaces;
using Admin.Application.Common.Results;

namespace Admin.Application.Auth.Commands.LogInToAdminPanel {
    public class LogInToAdminPanelCommand : IRequest<HandleResult<SecurityCredentialsDto>> {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LogInToAdminPanelCommandHandler : IRequestHandler<
        LogInToAdminPanelCommand, HandleResult<SecurityCredentialsDto>
    > {
        private readonly IAuthService _authService;

        public LogInToAdminPanelCommandHandler(IAuthService authService) {
            _authService = authService;
        }

        public async Task<HandleResult<SecurityCredentialsDto>> Handle(
            LogInToAdminPanelCommand command, CancellationToken cancellationToken
        ) {
            var outcome = await _authService.LogInAsAdmin(
                command.Email, command.Password
            );
            if (outcome.IsError) {
                return new HandleResult<SecurityCredentialsDto> {
                    Error = outcome.Error
                };
            }

            return new HandleResult<SecurityCredentialsDto> {
                Data = new SecurityCredentialsDto {
                    AccessToken = outcome.Data
                }
            };
        }
    }
}
