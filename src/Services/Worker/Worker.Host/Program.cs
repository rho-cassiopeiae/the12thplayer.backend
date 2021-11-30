using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using HostBuilder = Microsoft.Extensions.Hosting.Host;

using Quartz;

using Worker.Application.Jobs.OneOff;
using Worker.Application;
using Worker.Infrastructure;
using Worker.Host.Consumers.Admin;

namespace Worker.Host {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            HostBuilder.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    var configuration = hostContext.Configuration;

                    services.AddApplication();
                    services.AddInfrastructure(
                        configuration,
                        busCfg => {
                            busCfg.AddConsumer<JobCommandsConsumer>();
                        }
                    );

                    services.AddQuartz(quartzCfg => {
                        var quartzOptions = configuration
                            .GetSection("Quartz")
                            .Get<Options.QuartzOptions>();

                        quartzCfg.SetProperty(
                            "quartz.scheduler.interruptJobsOnShutdownWithWait", "true"
                        );
                        quartzCfg.UseMicrosoftDependencyInjectionJobFactory();
                        quartzCfg.UseSimpleTypeLoader();
                        quartzCfg.UseInMemoryStore();
                        quartzCfg.UseDefaultThreadPool(options => {
                            options.MaxConcurrency = quartzOptions.MaxConcurrency;
                        });

                        var applicationAssembly = Assembly.GetAssembly(
                            typeof(OneOffJob)
                        );

                        foreach (var job in quartzOptions.Jobs) {
                            var addJobMethod = typeof(ServiceCollectionExtensions)
                                .GetMethod(
                                    "AddJob",
                                    1,
                                    BindingFlags.Public | BindingFlags.Static,
                                    null,
                                    new[] {
                                        typeof(IServiceCollectionQuartzConfigurator),
                                        typeof(Action<IJobConfigurator>)
                                    },
                                    null
                                )
                                .MakeGenericMethod(
                                    applicationAssembly.GetType(job.Type)
                                );

                            Action<IJobConfigurator> jobConfigurator = config => {
                                config
                                    .WithIdentity(job.Name)
                                    .StoreDurably(true);

                                if (job.DataMap != null) {
                                    config.UsingJobData(new JobDataMap(
                                        (IDictionary<string, object>)
                                        new Dictionary<string, object>(
                                            job.DataMap.Select(kv =>
                                                new KeyValuePair<string, object>(
                                                    kv.Key, kv.Value
                                                )
                                            )
                                        )
                                    ));
                                }
                            };

                            addJobMethod.Invoke(
                                null,
                                new object[] { quartzCfg, jobConfigurator }
                            );
                        }
                    });

                    services.AddQuartzHostedService(options => {
                        options.WaitForJobsToComplete = true;
                    });
                });
    }
}
