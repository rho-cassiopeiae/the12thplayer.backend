using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using MediatR;

using Admin.Application.Common.Results;
using Admin.Application.Common.Attributes;
using Admin.Application.Common.Enums;
using Admin.Application.Common.Interfaces;
using JobDto = Admin.Application.Job.Common.Dto.Job;

namespace Admin.Application.Job.Commands.ExecuteOneOffJobs {
    [RequireAuthorization(Policy = "HasAdminPanelAccess")]
    [RequirePermission(
        Scope = PermissionScope.JobManagement,
        Flags = (int) JobManagementPermissions.ExecuteOneOffJobs
    )]
    public class ExecuteOneOffJobsCommand : IRequest<VoidResult> {
        public List<JobDto> Jobs { get; set; }
    }

    public class ExecuteOneOffJobsCommandHandler : IRequestHandler<
        ExecuteOneOffJobsCommand, VoidResult
    > {
        private readonly IJobScheduler _jobScheduler;

        public ExecuteOneOffJobsCommandHandler(IJobScheduler jobScheduler) {
            _jobScheduler = jobScheduler;
        }

        public async Task<VoidResult> Handle(
            ExecuteOneOffJobsCommand command, CancellationToken cancellationToken
        ) {
            await _jobScheduler.ExecuteOneOffJobs(command.Jobs);

            return VoidResult.Instance;
        }
    }
}
