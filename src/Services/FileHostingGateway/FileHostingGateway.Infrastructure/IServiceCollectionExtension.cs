using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;

using MessageBus.Components.HostedServices;

using FileHostingGateway.Application.Common.Interfaces;
using FileHostingGateway.Infrastructure.Vimeo;
using FileHostingGateway.Infrastructure.S3;

namespace FileHostingGateway.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IHostEnvironment hostEnvironment,
            IConfiguration configuration,
            Action<IServiceCollectionBusConfigurator> busCfgCallback
        ) {
            // @@TODO: Configure Polly.
            services.AddHttpClient("vimeo", (sp, client) => {
                var configuration = sp.GetRequiredService<IConfiguration>();
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.vimeo.*+json;version=3.4");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + configuration["Vimeo:ApiToken"]);
            });
            services.AddHttpClient("vimeo-old", (sp, client) => {
                var configuration = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["Vimeo:BaseAddressOld"]);
            });

            services.AddSingleton<IVimeoGateway, VimeoGateway>();

            if (hostEnvironment.IsDevelopment()) {
                services.AddSingleton<IS3Gateway, S3GatewayMock>();
            } else {
                services.AddSingleton<IS3Gateway, S3Gateway>();
            }

            services.AddMassTransit(busCfg => {
                busCfgCallback(busCfg);

                busCfg.UsingRabbitMq((context, rabbitCfg) => {
                    rabbitCfg.Host(configuration["RabbitMQ:Host"]);

                    rabbitCfg.ConfigureEndpoints(
                        context,
                        new KebabCaseEndpointNameFormatter("file-hosting-gateway", false)
                    );
                });
            });

            services.AddHostedService<MassTransitBusController>();

            return services;
        }
    }
}
