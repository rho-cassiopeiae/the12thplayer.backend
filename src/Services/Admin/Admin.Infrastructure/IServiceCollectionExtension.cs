using System;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using MassTransit;

using MessageBus.Components.HostedServices;
using MessageBus.Contracts.Requests.Admin;

using Admin.Application.Common.Interfaces;
using Admin.Infrastructure.Auth;
using Admin.Infrastructure.Identity;

namespace Admin.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
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

            services.AddAuthorizationCore(options => {
                options.AddPolicy(
                    "HasAdminPanelAccess",
                    builder => builder.RequireClaim("__Admin")
                );
            });

            services.AddScoped<IAuthenticationContext, AuthenticationContext>();
            services.AddTransient<IAuthorizationService, AuthorizationService>();

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
