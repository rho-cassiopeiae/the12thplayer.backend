using System.Collections.Generic;
using System.Threading.Tasks;

using JobDto = Admin.Application.Job.Common.Dto.Job;

namespace Admin.Application.Common.Interfaces {
    public interface IJobScheduler {
        Task ExecuteOneOffJobs(List<JobDto> jobs);
        Task SchedulePeridiocJobs(List<JobDto> jobs);
    }
}
