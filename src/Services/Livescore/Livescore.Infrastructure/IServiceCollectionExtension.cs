using System;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;

using MessageBus.Components.HostedServices;

using Livescore.Infrastructure.Persistence;
using Livescore.Domain.Aggregates.Country;
using Livescore.Infrastructure.Persistence.Repositories;
using Livescore.Domain.Aggregates.Team;
using Livescore.Domain.Aggregates.Venue;
using Livescore.Domain.Aggregates.Manager;

namespace Livescore.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IServiceCollectionBusConfigurator> busCfgCallback
        ) {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddDbContext<LivescoreDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(
                    configuration.GetConnectionString("Livescore"),
                    pgOptionsBuilder => pgOptionsBuilder.MigrationsHistoryTable(
                        "__EFMigrationsHistory", "livescore"
                    )
                )
            );

            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IVenueRepository, VenueRepository>();
            services.AddScoped<IManagerRepository, ManagerRepository>();

            services.AddMassTransit(busCfg => {
                busCfgCallback(busCfg);

                busCfg.UsingRabbitMq((context, rabbitCfg) => {
                    rabbitCfg.Host("rabbit");

                    rabbitCfg.ConfigureEndpoints(
                        context,
                        new KebabCaseEndpointNameFormatter("livescore", false)
                    );
                });
            });

            services.AddHostedService<MassTransitBusController>();

            return services;
        }
    }
}
