using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using MediatR;
using FluentValidation;

using Livescore.Application.Common.Behaviors;

namespace Livescore.Application {
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
