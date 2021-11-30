using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using Admin.Application.Common.Results;
using Admin.Application.Job.Commands.ExecuteOneOffJobs;

namespace Admin.Api.Controllers {
    [Route("admin/jobs")]
    public class JobController {
        private readonly ISender _mediator;

        public JobController(ISender mediator) {
            _mediator = mediator;
        }

        [HttpPost("one-off")]
        public Task<VoidResult> ExecuteOneOffJobs(
            [FromBody] ExecuteOneOffJobsCommand command
        ) => _mediator.Send(command);
    }
}
