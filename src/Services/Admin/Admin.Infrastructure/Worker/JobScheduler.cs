using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using MessageBus.Contracts.Commands.Admin;

using Admin.Application.Common.Interfaces;
using Admin.Application.Job.Common.Dto;

namespace Admin.Infrastructure.Worker {
    public class JobScheduler : IJobScheduler {
        private readonly IBus _bus;

        public JobScheduler(IBus bus) {
            _bus = bus;
        }

        public async Task ExecuteOneOffJobs(List<JobDto> jobs) {
            await _bus.Send(new ExecuteOneOffJobs {
                CorrelationId = Guid.NewGuid(),
                Jobs = jobs.Select(job => new ExecuteOneOffJobs.Job {
                    Name = job.Name,
                    Type = job.Type,
                    DataMap = job.DataMap != null ?
                        new Dictionary<string, string>(
                            job.DataMap.Select(kv =>
                                new KeyValuePair<string, string>(
                                    kv.Key, kv.Value.ToString()
                                )
                            )
                        ) :
                        null,
                    ExecuteAfter = job.ExecuteAfter
                })
            });
        }

        public async Task SchedulePeridiocJobs(List<JobDto> jobs) {
            await _bus.Send(new SchedulePeriodicJobs {
                CorrelationId = Guid.NewGuid(),
                Jobs = jobs.Select(job => new SchedulePeriodicJobs.Job {
                    Name = job.Name,
                    Type = job.Type,
                    CronSchedule = job.CronSchedule,
                    DataMap = job.DataMap != null ?
                        new Dictionary<string, string>(
                            job.DataMap.Select(kv =>
                                new KeyValuePair<string, string>(
                                    kv.Key, kv.Value.ToString()
                                )
                            )
                        ) :
                        null
                })
            });
        }
    }
}
