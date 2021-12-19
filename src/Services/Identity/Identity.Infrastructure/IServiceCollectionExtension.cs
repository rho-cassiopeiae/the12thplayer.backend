using System;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;

using MessageBus.Components.HostedServices;
using MessageBus.Contracts.Requests.Identity;

using Identity.Infrastructure.Account;
using Identity.Infrastructure.Integration;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Models;
using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Integration;
using Identity.Infrastructure.Persistence.Repositories;
using Identity.Domain.Base;

namespace Identity.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IServiceCollectionBusConfigurator> busCfgCallback
        ) {
            services.AddDbContext<UserDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(
                    configuration.GetConnectionString("Identity"),
                    pgOptionsBuilder => pgOptionsBuilder.MigrationsHistoryTable(
                        "__EFMigrationsHistory_UserDbContext", "identity"
                    )
                )
            );

            services.AddDbContext<IntegrationEventDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(
                    configuration.GetConnectionString("Identity"),
                    pgOptionsBuilder => pgOptionsBuilder.MigrationsHistoryTable(
                        "__EFMigrationsHistory_IntegrationEventDbContext", "identity"
                    )
                )
            );

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services
                .AddIdentityCore<User>(options => {
                    options.Tokens.EmailConfirmationTokenProvider = "ConfirmationCodeProvider";
                    options.Tokens.PasswordResetTokenProvider = "ConfirmationCodeProvider";
                    options.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub;
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._";
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.SignIn.RequireConfirmedAccount = true;
                })
                .AddEntityFrameworkStores<UserDbContext>()
                .AddDefaultTokenProviders()
                .AddConfirmationCodeProvider();

            services.AddDataProtection();
            services.AddHttpContextAccessor();
            services.TryAddScoped<SignInManager<User>>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.TryAddScoped<IUserService, UserService>();
            services.AddSingleton<ISecurityTokenProvider, SecurityTokenProvider>();
            services.AddSingleton<IPrincipalDataProvider, PrincipalDataProvider>();
            services.AddScoped<IProfileSvcQueryable, ProfileSvcQueryable>();

            services.AddScoped<IIntegrationEventRepository, IntegrationEventRepository>();

            services.AddTransient<IIntegrationEventPublisher, IntegrationEventPublisher>();
            services.AddTransient<IIntegrationEventTracker, IntegrationEventTracker>();

            services.AddMassTransit(busCfg => {
                busCfgCallback(busCfg);

                busCfg.AddRequestClient<GetProfilePermissions>(
                    new Uri("queue:profile-permission-requests")
                );

                busCfg.UsingRabbitMq((context, rabbitCfg) => {
                    rabbitCfg.Host(configuration["RabbitMQ:Host"]);

                    rabbitCfg.ConfigureEndpoints(
                        context,
                        new KebabCaseEndpointNameFormatter("identity", false)
                    );
                });
            });

            services.AddHostedService<MassTransitBusController>();

            return services;
        }
    }
}
