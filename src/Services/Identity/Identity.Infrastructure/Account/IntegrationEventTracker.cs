using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Npgsql;

namespace Identity.Infrastructure.Account {
    public class IntegrationEventTracker : IIntegrationEventTracker {
        private readonly ILogger<IntegrationEventTracker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _connectionString;
        private NpgsqlConnection _connection;

        public IntegrationEventTracker(
            ILogger<IntegrationEventTracker> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration
        ) {
            _logger = logger;
            _serviceProvider = serviceProvider;
            // @@NOTE: Connection idle lifetime is 300s by default. Add keep-alive
            // to the default connection string to keep connection open.
            _connectionString = configuration.GetConnectionString("Identity") +
                "KeepAlive=150";
        }

        private NpgsqlConnection _ensureConnection() =>
            _connection ?? (_connection = new NpgsqlConnection(_connectionString));

        private async ValueTask _ensureConnectionOpen() {
            if (_connection.State != ConnectionState.Open) {
                await _connection.OpenAsync();
            }
        }

        public async ValueTask DisposeAsync() {
            if (_connection != null) {
                await _connection.DisposeAsync();
            }
        }

        public async Task ListenForAndPublishNewEvents(
            CancellationToken stoppingToken
        ) {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            _ensureConnection();
            await _ensureConnectionOpen();

            _connection.Notification += (_, eventArgs) =>
                _fetchAndPublishEventById(Guid.Parse(eventArgs.Payload));

            _connection.StateChange += (_, eventArgs) => {
                if (eventArgs.CurrentState is
                    ConnectionState.Closed or ConnectionState.Broken
                ) {
                    if (!cts.IsCancellationRequested) {
                        cts.Cancel();
                    }
                }
            };

            await using (var cmd = new NpgsqlCommand(
                "LISTEN integration_event_channel", _connection
            )) {
                await cmd.ExecuteNonQueryAsync();
            }

            while (true) {
                await _connection.WaitAsync(cts.Token);
            }
        }

        private async void _fetchAndPublishEventById(Guid eventId) {
            try {
                await using var publisher =
                    _serviceProvider.GetRequiredService<IIntegrationEventPublisher>();

                await publisher.FetchAndPublishEventById(eventId);
            } catch (Exception e) {
                _logger.LogError(
                    e,
                    "Error publishing integration event with id {EventId}",
                    eventId
                );
            }
        }
    }
}
