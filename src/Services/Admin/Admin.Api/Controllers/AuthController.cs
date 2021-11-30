using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using Admin.Application.Auth.Commands.LogInToAdminPanel;
using Admin.Application.Common.Results;

namespace Admin.Api.Controllers {
    [Route("admin")]
    public class AuthController {
        private readonly ISender _mediator;

        public AuthController(ISender mediator) {
            _mediator = mediator;
        }

        [HttpPost("log-in")]
        public Task<HandleResult<SecurityCredentials>> LogIn(
            [FromBody] LogInToAdminPanelCommand command
        ) => _mediator.Send(command);
    }
}
