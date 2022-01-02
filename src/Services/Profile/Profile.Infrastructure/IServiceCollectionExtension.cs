using System;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;

using MessageBus.Components.HostedServices;
using MessageBus.Contracts.Requests.Profile;

using Profile.Infrastructure.Persistence;
using Profile.Domain.Aggregates.Profile;
using Profile.Infrastructure.Persistence.Repositories;
using Profile.Application.Common.Interfaces;
using Profile.Infrastructure.Persistence.Queryables;
using Profile.Infrastructure.Identity;
using Profile.Domain.Base;
using Profile.Application.Common.Integration;
using Profile.Infrastructure.Integration;
using Profile.Infrastructure.FileUpload;

namespace Profile.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IServiceCollectionBusConfigurator> busCfgCallback
        ) {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    var rsa = RSA.Create(); // @@NOTE: Important to not dispose.
                    rsa.FromXmlString(configuration["Jwt:PublicKey"]);

                    options.MapInboundClaims = false;
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new RsaSecurityKey(rsa)
                    };
                    options.Events = new JwtBearerEvents {
                        OnTokenValidated = context => {
                            var authenticationContext = context
                                .HttpContext
                                .RequestServices
                                .GetRequiredService<IAuthenticationContext>();

                            authenticationContext.User = context.Principal;

                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context => {
                            var authenticationContext = context
                                .HttpContext
                                .RequestServices
                                .GetRequiredService<IAuthenticationContext>();

                            authenticationContext.Failure = context.Exception;

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorizationCore();

            services.AddScoped<IAuthenticationContext, AuthenticationContext>();
            services.AddTransient<IAuthorizationService, AuthorizationService>();
            services.AddSingleton<IPrincipalDataProvider, PrincipalDataProvider>();

            services.AddDbContext<ProfileDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(
                    configuration.GetConnectionString("Profile"),
                    pgOptionsBuilder => pgOptionsBuilder.MigrationsHistoryTable(
                        "__EFMigrationsHistory_ProfileDbContext", "profile"
                    )
                )
            );

            services.AddDbContext<IntegrationEventDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(
                    configuration.GetConnectionString("Profile"),
                    pgOptionsBuilder => pgOptionsBuilder.MigrationsHistoryTable(
                        "__EFMigrationsHistory_IntegrationEventDbContext", "profile"
                    )
                )
            );

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IProfileQueryable, ProfileQueryable>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IIntegrationEventRepository, IntegrationEventRepository>();

            services.AddTransient<IIntegrationEventPublisher, IntegrationEventPublisher>();
            services.AddTransient<IIntegrationEventTracker, IntegrationEventTracker>();

            services.AddSingleton<MultipartRequestHelper>();
            services.AddSingleton<ImageFileValidator>();
            services.AddSingleton<IFileReceiver, FileReceiver>();
            services.TryAddScoped<IFileHosting, FileHosting>();

            services.AddMassTransit(busCfg => {
                busCfgCallback(busCfg);

                busCfg.AddRequestClient<UploadImage>(new Uri("queue:file-hosting-gateway-upload-requests"));

                busCfg.UsingRabbitMq((context, rabbitCfg) => {
                    rabbitCfg.Host(configuration["RabbitMQ:Host"]);

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
