using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MassTransit;

using MessageBus.Components.HostedServices;
using MessageBus.Contracts.Requests.Admin;

using Admin.Application.Common.Interfaces;
using Admin.Infrastructure.Auth;

namespace Admin.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        ) {
            services.AddTransient<IAuthService, AuthService>();

            services.AddMassTransit(busCfg => {
                busCfg.AddRequestClient<LogInAsAdmin>(
                    new Uri("queue:identity-auth-requests")
                );

                busCfg.UsingRabbitMq((context, rabbitCfg) => {
                    rabbitCfg.Host("rabbit");
                });
            });

            services.AddHostedService<MassTransitBusController>();

            return services;
        }
    }
}
