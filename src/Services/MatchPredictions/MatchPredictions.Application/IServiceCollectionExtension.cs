using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using MediatR;
using FluentValidation;

using MatchPredictions.Application.Common.Behaviors;

namespace MatchPredictions.Application {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddApplication(this IServiceCollection services) {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
