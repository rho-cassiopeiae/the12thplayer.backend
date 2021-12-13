using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;

using MessageBus.Components.HostedServices;

using FileHostingGateway.Application.Common.Interfaces;
using FileHostingGateway.Infrastructure.Vimeo;

namespace FileHostingGateway.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
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

            services.AddMassTransit(busCfg => {
                busCfgCallback(busCfg);

                busCfg.UsingRabbitMq((context, rabbitCfg) => {
                    rabbitCfg.Host("rabbit");

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
