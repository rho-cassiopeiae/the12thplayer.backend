﻿using System;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;

using MessageBus.Components.HostedServices;

using Worker.Application.Common.Interfaces;
using Worker.Infrastructure.FootballDataProvider;
using Worker.Infrastructure.Livescore;

namespace Worker.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IServiceCollectionBusConfigurator> busCfgCallback
        ) {
            services.AddSingleton<Mapper>();
            services.AddSingleton<IFootballDataProvider, SportmonksDataProvider>();

            services.AddScoped<ILivescoreSeeder, LivescoreSeeder>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMassTransit(busCfg => {
                busCfgCallback(busCfg);

                busCfg.UsingRabbitMq((context, rabbitCfg) => {
                    rabbitCfg.Host("rabbit");

                    rabbitCfg.ConfigureEndpoints(
                        context,
                        new KebabCaseEndpointNameFormatter("worker", false)
                    );
                });
            });

            services.AddHostedService<MassTransitBusController>();

            return services;
        }
    }
}
