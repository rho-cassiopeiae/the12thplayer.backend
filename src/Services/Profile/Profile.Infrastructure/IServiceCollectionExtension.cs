using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;

using MessageBus.Components.HostedServices;

using Profile.Infrastructure.Persistence;
using Profile.Domain.Aggregates.Profile;
using Profile.Infrastructure.Persistence.Repositories;
using Profile.Application.Common.Interfaces;
using Profile.Infrastructure.Persistence.Queryables;

namespace Profile.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IServiceCollectionBusConfigurator> busCfgCallback
        ) {
            services.AddDbContext<ProfileDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(
                    configuration.GetConnectionString("Profile"),
                    pgOptionsBuilder => pgOptionsBuilder.MigrationsHistoryTable(
                        "__EFMigrationsHistory", "profile"
                    )
                )
            );

            services.AddScoped<IProfileQueryable, ProfileQueryable>();
            services.AddScoped<IProfileRepository, ProfileRepository>();

            services.AddMassTransit(busCfg => {
                busCfgCallback(busCfg);

                busCfg.UsingRabbitMq((context, rabbitCfg) => {
                    rabbitCfg.Host("rabbit");

                    rabbitCfg.ConfigureEndpoints(
                        context,
                        new KebabCaseEndpointNameFormatter("profile", false)
                    );
                });
            });

            services.AddHostedService<MassTransitBusController>();

            return services;
        }
    }
}
