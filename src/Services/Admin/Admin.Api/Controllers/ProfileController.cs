using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using Admin.Application.Common.Results;
using Admin.Application.Profile.Commands.GrantSuperuserPermissions;

namespace Admin.Api.Controllers {
    [Route("admin/profiles")]
    public class ProfileController {
        private readonly ISender _mediator;

        public ProfileController(ISender mediator) {
            _mediator = mediator;
        }

        [HttpPut("superuser")]
        public Task<VoidResult> GrantSuperuserPermissions(
            [FromBody] GrantSuperuserPermissionsCommand command
        ) => _mediator.Send(command);
    }
}
