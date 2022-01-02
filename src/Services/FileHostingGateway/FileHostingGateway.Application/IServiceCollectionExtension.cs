using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using MediatR;

namespace FileHostingGateway.Application {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddApplication(this IServiceCollection services) {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
