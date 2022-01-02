using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using MediatR;

using Livescore.Application.Common.Behaviors;

namespace Livescore.Application {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddApplication(this IServiceCollection services) {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

            return services;
        }
    }
}
