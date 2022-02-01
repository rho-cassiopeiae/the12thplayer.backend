using FluentValidation;

namespace Admin.Application.Job.Commands.ExecuteOneOffJobs {
    public class ExecuteOneOffJobsCommandValidator : AbstractValidator<ExecuteOneOffJobsCommand> {
        public ExecuteOneOffJobsCommandValidator() {
            RuleFor(c => c.Jobs)
                .NotEmpty()
                .Must(jobs => {
                    var valid = true;
                    for (int i = 0; i < jobs.Count; ++i) {
                        var job = jobs[i];
                        valid = !string.IsNullOrWhiteSpace(job.Name) &&
                            !string.IsNullOrWhiteSpace(job.Type) &&
                            job.CronSchedule == null &&
                            i == 0 ? job.ExecuteAfter == null : !string.IsNullOrWhiteSpace(job.ExecuteAfter);

                        if (!valid) {
                            break;
                        }
                    }

                    return valid;
                });
        }
    }
}
