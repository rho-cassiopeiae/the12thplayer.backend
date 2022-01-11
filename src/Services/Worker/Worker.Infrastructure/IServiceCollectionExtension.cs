using System;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;

using MessageBus.Components.HostedServices;
using MessageBus.Contracts.Requests.Worker;

using Worker.Application.Common.Interfaces;
using Worker.Infrastructure.FootballDataProvider;
using Worker.Infrastructure.Livescore;
using Worker.Infrastructure.MatchPredictions;

namespace Worker.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IServiceCollectionBusConfigurator> busCfgCallback
        ) {
            services.AddSingleton<FootballDataProvider.Mapper>();
            services.AddSingleton<IFootballDataProvider, SportmonksDataProvider>();

            services.AddScoped<ILivescoreSeeder, LivescoreSeeder>();
            services.AddScoped<IFixtureLivescoreNotifier, FixtureLivescoreNotifier>();
            services.AddScoped<ILivescoreSvcQueryable, LivescoreSvcQueryable>();
            services.AddScoped<IFileHostingSeeder, FileHostingSeeder>();
            services.AddScoped<IMatchPredictionsSeeder, MatchPredictionsSeeder>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMassTransit(busCfg => {
                busCfgCallback(busCfg);

                busCfg.AddRequestClient<AddFileFoldersForFixture>(
                    new Uri("queue:file-hosting-gateway-folder-requests")
                );

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
