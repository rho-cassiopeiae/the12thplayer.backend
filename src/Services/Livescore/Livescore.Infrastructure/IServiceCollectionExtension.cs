﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
using Livescore.Infrastructure.InMemory.Queryables;
using Livescore.Domain.Aggregates.UserVote;
using Livescore.Domain.Aggregates.VideoReaction;
using Livescore.Infrastructure.Identity;

namespace Livescore.Infrastructure {
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
                        OnMessageReceived = context => {
                            var accessToken = context.Request.Query["access_token"];
                            if (
                                !string.IsNullOrEmpty(accessToken) &&
                                context.Request.Path.StartsWithSegments("/livescore")
                            ) {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context => {
                            var authenticationContext = context
                                .HttpContext
                                .RequestServices
                                .GetRequiredService<IAuthenticationContext>();

                            authenticationContext.User = context.Principal;

                            if (context.Request.Path.StartsWithSegments("/livescore")) {
                                authenticationContext.Token = context.SecurityToken;
                            }

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
                bool allowAdmin = configuration.GetValue("Redis:AllowAdmin", false);

                return ConnectionMultiplexer.Connect(
                    $"{host}:{port}" + (allowAdmin ? ",allowAdmin=true" : string.Empty)
                );
            });

            services.AddScoped<IInMemUnitOfWork, InMemUnitOfWork>();
            services.AddScoped<IDiscussionInMemRepository, DiscussionInMemRepository>();
            services.AddScoped<IFixtureLivescoreStatusInMemRepository, FixtureLivescoreStatusInMemRepository>();
            services.AddScoped<IPlayerRatingInMemRepository, PlayerRatingInMemRepository>();
            services.AddScoped<IVideoReactionInMemRepository, VideoReactionInMemRepository>();
            services.AddScoped<IUserVoteInMemRepository, UserVoteInMemRepository>();

            services.AddScoped<IPlayerRatingInMemQueryable, PlayerRatingInMemQueryable>();
            services.AddScoped<IVideoReactionInMemQueryable, VideoReactionInMemQueryable>();

            services.AddMassTransit(busCfg => {
                busCfgCallback(busCfg);

                busCfg.UsingRabbitMq((context, rabbitCfg) => {
                    rabbitCfg.Host(configuration["RabbitMQ:Host"]);

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
