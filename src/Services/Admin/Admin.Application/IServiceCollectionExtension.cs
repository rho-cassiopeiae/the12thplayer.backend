using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using MediatR;

using Admin.Application.Common.Behaviors;

namespace Admin.Application {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddApplication(
            this IServiceCollection services
        ) {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(
                typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>)
            );

            return services;
        }
    }
}
