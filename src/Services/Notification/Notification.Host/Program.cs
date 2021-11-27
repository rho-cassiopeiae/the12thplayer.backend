using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HostBuilder = Microsoft.Extensions.Hosting.Host;

using MassTransit;

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
                            rabbitCfg.Host("rabbit");

                            rabbitCfg.ConfigureEndpoints(context);
                        });
                    });

                    services.AddHostedService<MassTransitBusController>();
                });
    }
}
