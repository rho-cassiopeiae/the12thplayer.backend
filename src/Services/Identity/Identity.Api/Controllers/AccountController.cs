using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using Identity.Application.Account.Commands.SignUp;
using Identity.Application.Account.Commands.ConfirmEmail;
using Identity.Application.Common.Results;
using Identity.Application.Account.Commands.SignIn;
using Identity.Application.Account.Common.Dto;
using Identity.Application.Account.Commands.RefreshAccessToken;

namespace Identity.Api.Controllers {
    [Route("identity/[controller]")]
    public class AccountController : ControllerBase {
        private readonly ISender _mediator;

        public AccountController(ISender mediator) {
            _mediator = mediator;
        }

        [HttpPost("sign-up")]
        public Task<VoidResult> SignUp([FromBody] SignUpCommand command) =>
            _mediator.Send(command);

        [HttpPost("confirm-email")]
        public Task<VoidResult> ConfirmEmail([FromBody] ConfirmEmailCommand command) =>
            _mediator.Send(command);

        [HttpPost("sign-in")]
        public Task<HandleResult<SecurityCredentialsDto>> SignIn([FromBody] SignInCommand command) =>
            _mediator.Send(command);

        [HttpPost("refresh-access-token")]
        public Task<HandleResult<SecurityCredentialsDto>> RefreshAccessToken(
            [FromBody] RefreshAccessTokenCommand command
        ) => _mediator.Send(command);
    }
}
