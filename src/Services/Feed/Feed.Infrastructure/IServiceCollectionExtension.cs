using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;

using MessageBus.Components.HostedServices;

using Feed.Infrastructure.Persistence;
using Feed.Domain.Base;
using Feed.Domain.Aggregates.Author;
using Feed.Infrastructure.Persistence.Repositories;

namespace Feed.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IServiceCollectionBusConfigurator> busCfgCallback
        ) {
            services.AddDbContext<FeedDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(
                    configuration.GetConnectionString("Feed"),
                    pgOptionsBuilder => pgOptionsBuilder.MigrationsHistoryTable(
                        "__EFMigrationsHistory_FeedDbContext", "feed"
                    )
                )
            );

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAuthorRepository, AuthorRepository>();

            services.AddMassTransit(busCfg => {
                busCfgCallback(busCfg);

                busCfg.UsingRabbitMq((context, rabbitCfg) => {
                    rabbitCfg.Host(configuration["RabbitMQ:Host"]);

                    rabbitCfg.ConfigureEndpoints(
                        context,
                        new KebabCaseEndpointNameFormatter("feed", false)
                    );
                });
            });

            services.AddHostedService<MassTransitBusController>();

            return services;
        }
    }
}
