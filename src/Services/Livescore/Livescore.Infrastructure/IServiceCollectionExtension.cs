using System;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using StackExchange.Redis;

using MessageBus.Components.HostedServices;

using Livescore.Infrastructure.Persistence;
using Livescore.Domain.Aggregates.Country;
using Livescore.Infrastructure.Persistence.Repositories;
using Livescore.Domain.Aggregates.Team;
using Livescore.Domain.Aggregates.Venue;
using Livescore.Domain.Aggregates.Manager;
using Livescore.Domain.Aggregates.League;
using Livescore.Domain.Aggregates.Player;
using Livescore.Domain.Aggregates.Fixture;
using Livescore.Application.Common.Interfaces;
using Livescore.Infrastructure.Persistence.Queryables;
using Livescore.Domain.Aggregates.Discussion;
using Livescore.Infrastructure.InMemory.Repositories;
using Livescore.Domain.Aggregates.FixtureLivescoreStatus;
using Livescore.Domain.Base;
using Livescore.Infrastructure.InMemory;
using Livescore.Domain.Aggregates.PlayerRating;

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
                        "__EFMigrationsHistory_LivescoreDbContext", "livescore"
                    )
                )
            );

            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IVenueRepository, VenueRepository>();
            services.AddScoped<IManagerRepository, ManagerRepository>();
            services.AddScoped<ILeagueRepository, LeagueRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IFixtureRepository, FixtureRepository>();

            services.AddScoped<ILivescoreQueryable, LivescoreQueryable>();

            services.AddSingleton(sp => {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var host = configuration["Redis:Host"];
                var port = configuration["Redis:Port"];

                return ConnectionMultiplexer.Connect($"{host}:{port}");
            });

            services.AddScoped<IInMemUnitOfWork, InMemUnitOfWork>();
            services.AddScoped<IDiscussionInMemRepository, DiscussionInMemRepository>();
            services.AddScoped<IFixtureLivescoreStatusInMemRepository, FixtureLivescoreStatusInMemRepository>();
            services.AddScoped<IPlayerRatingInMemRepository, PlayerRatingInMemRepository>();

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
