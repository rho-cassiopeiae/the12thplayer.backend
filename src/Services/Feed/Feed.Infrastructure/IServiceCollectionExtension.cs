using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;

using MessageBus.Contracts.Requests.Feed;
using MessageBus.Components.HostedServices;

using Feed.Infrastructure.Persistence;
using Feed.Domain.Base;
using Feed.Domain.Aggregates.Author;
using Feed.Infrastructure.Persistence.Repositories;
using Feed.Domain.Aggregates.Article;
using Feed.Domain.Aggregates.UserVote;
using Feed.Domain.Aggregates.Comment;
using Feed.Application.Common.Interfaces;
using Feed.Infrastructure.Persistence.Queryables;
using Feed.Infrastructure.Identity;
using Feed.Infrastructure.FileUpload;

namespace Feed.Infrastructure {
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
                    options.RequireHttpsMetadata = false; // @@TODO: Depend on environment.
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
                            if (context.Request.Path.StartsWithSegments("/feed/hub")) {
                                var accessToken = context.Request.Query["access_token"];
                                if (!string.IsNullOrEmpty(accessToken)) {
                                    context.Token = accessToken;
                                }
                            }

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context => {
                            var authenticationContext = context
                                .HttpContext
                                .RequestServices
                                .GetRequiredService<IAuthenticationContext>();

                            authenticationContext.User = context.Principal;

                            if (context.Request.Path.StartsWithSegments("/feed/hub")) {
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

            services.AddScoped<FeedDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IUserVoteRepository, UserVoteRepository>();

            services.AddScoped<ICommentQueryable, CommentQueryable>();
            services.AddScoped<IArticleQueryable, ArticleQueryable>();
            services.AddScoped<IUserVoteQueryable, UserVoteQueryable>();

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
                        new KebabCaseEndpointNameFormatter("feed", false)
                    );
                });
            });

            services.AddHostedService<MassTransitBusController>();

            return services;
        }
    }
}
