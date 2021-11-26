using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using Identity.Application.Account.Commands.SignUp;
using Identity.Application.Account.Commands.ConfirmEmail;
using Identity.Application.Common.Results;

namespace Identity.Api.Controllers {
    [Route("[controller]")]
    public class AccountController : ControllerBase {
        private readonly ISender _mediator;

        public AccountController(ISender mediator) {
            _mediator = mediator;
        }

        [HttpPost("sign-up")]
        public Task<VoidResult> SignUp([FromBody] SignUpCommand command)
            => _mediator.Send(command);

        [HttpPost("confirm-email")]
        public Task<HandleResult<SecurityCredentials>> ConfirmEmail(
            [FromBody] ConfirmEmailCommand command
        ) => _mediator.Send(command);
    }
}
