using System.Collections.Generic;
using System.Threading.Tasks;

using Admin.Application.Job.Common.Dto;

namespace Admin.Application.Common.Interfaces {
    public interface IJobScheduler {
        Task ExecuteOneOffJobs(List<JobDto> jobs);
        Task SchedulePeridiocJobs(List<JobDto> jobs);
    }
}
