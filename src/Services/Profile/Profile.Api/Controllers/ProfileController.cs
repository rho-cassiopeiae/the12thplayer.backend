using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using Profile.Application.Common.Results;
using Profile.Application.Profile.Commands.UpdateProfileImage;
using Profile.Api.Controllers.Filters;

namespace Profile.Api.Controllers {
    [Route("[controller]")]
    public class ProfileController : ControllerBase {
        private readonly ISender _mediator;
        public ProfileController(ISender mediator) {
            _mediator = mediator;
        }

        [DisableFormValueModelBinding]
        [RequestSizeLimit(2 * 1024 * 1024)] // @@TODO: Config.
        [HttpPost("update-profile-image")]
        public Task<HandleResult<string>> UpdateProfileImage() => _mediator.Send(
            new UpdateProfileImageCommand {
                Request = HttpContext.Request
            }
        );
    }
}
