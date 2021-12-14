using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Profile.Infrastructure.Integration;

namespace Profile.Api.HostedServices {
    public class IntegrationEventDispatcher : BackgroundService {
        private readonly ILogger<IntegrationEventDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;

        public IntegrationEventDispatcher(
            ILogger<IntegrationEventDispatcher> logger,
            IServiceProvider serviceProvider
        ) {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            var timer = new Timer(
                async _ => {
                    try {
                        await using var publisher = _serviceProvider.GetRequiredService<IIntegrationEventPublisher>();
                        await publisher.FetchAndPublishPendingEvents();
                    } catch (Exception e) {
                        _logger.LogWarning(
                            e,
                            "Error publishing pending integration events"
                        );
                    }
                },
                null,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1)
            );

            stoppingToken.Register(() => timer.Dispose());

            while (!stoppingToken.IsCancellationRequested) {
                try {
                    await using var tracker = _serviceProvider.GetRequiredService<IIntegrationEventTracker>();
                    await tracker.ListenForAndPublishNewEvents(stoppingToken);
                } catch (Exception e) {
                    _logger.LogWarning(
                        e,
                        "Error listening for new notifications; Connection closed or broken"
                    );
                }
            }
        }
    }
}
