using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HostBuilder = Microsoft.Extensions.Hosting.Host;

using MassTransit;
using MassTransit.Definition;

using MessageBus.Components.HostedServices;

using Notification.Application;
using Notification.Host.Consumers.Identity;

namespace Notification.Host {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            HostBuilder.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    services.AddApplication();

                    services.AddMassTransit(busCfg => {
                        busCfg.AddConsumer<UserAccountEventsConsumer>();

                        busCfg.UsingRabbitMq((context, rabbitCfg) => {
                            rabbitCfg.Host(hostContext.Configuration["RabbitMQ:Host"]);

                            rabbitCfg.ConfigureEndpoints(
                                context,
                                new KebabCaseEndpointNameFormatter("notification", false)
                            );
                        });
                    });

                    services.AddHostedService<MassTransitBusController>();
                });
    }
}
