using System;
using System.Threading;
using System.Threading.Tasks;

using Identity.Infrastructure.Account;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Api.HostedServices {
    public class IntegrationEventDispatcher : BackgroundService {
        private readonly IServiceProvider _serviceProvider;

        public IntegrationEventDispatcher(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                await using var tracker =
                    _serviceProvider.GetRequiredService<IIntegrationEventTracker>();

                await tracker.ListenForAndPublishNewEvents(stoppingToken);
            }
        }
    }
}
