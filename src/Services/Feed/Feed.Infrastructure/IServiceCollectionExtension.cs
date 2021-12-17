using System;

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
using Feed.Domain.Aggregates.Article;
using Feed.Domain.Aggregates.UserVote;
using Feed.Domain.Aggregates.Comment;
using Feed.Application.Common.Interfaces;
using Feed.Infrastructure.Persistence.Queryables;

namespace Feed.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IServiceCollectionBusConfigurator> busCfgCallback
        ) {
            services.AddScoped<FeedDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IUserVoteRepository, UserVoteRepository>();

            services.AddScoped<ICommentQueryable, CommentQueryable>();
            services.AddScoped<IArticleQueryable, ArticleQueryable>();

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
