using System.Text;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using Npgsql;

using Identity.Infrastructure.Account.Persistence;
using Identity.Infrastructure.Account.Persistence.Models;

namespace Identity.Infrastructure.Account {
    public class IntegrationEventPublisher : IIntegrationEventPublisher {
        private readonly string _connectionString;
        private NpgsqlConnection _connection;

        public IntegrationEventPublisher(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString("Identity");
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

        public async Task FetchAndPublishPendingEvents() {
            _ensureConnection();
            await _ensureConnectionOpen();

            await using var txn =
                await _connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            List<IntegrationEvent> events = null;

            await using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = _connection;

                cmd.Parameters.Add(
                    new NpgsqlParameter<int>(
                        nameof(IntegrationEvent.Status),
                        (int) IntegrationEventStatus.Pending
                    )
                );

                cmd.CommandText = $@"
                    SELECT
                        ""{nameof(IntegrationEvent.Id)}"",
                        ""{nameof(IntegrationEvent.Type)}"",
                        ""{nameof(IntegrationEvent.Payload)}""
                    FROM
                        identity.""{nameof(UserDbContext.IntegrationEvents)}""
                    WHERE
                        ""{nameof(IntegrationEvent.Status)}"" =
                            @{cmd.Parameters.First().ParameterName}
                    FOR NO KEY UPDATE;
                ";

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) {
                    int i = 0;
                    events ??= new List<IntegrationEvent>();
                    events.Add(new IntegrationEvent {
                        Id = reader.GetInt32(i),
                        Type = (IntegrationEventType) reader.GetInt32(++i),
                        Payload = reader.GetFieldValue<JsonDocument>(++i),
                        Status = IntegrationEventStatus.Pending
                    });
                }
            }

            if (events != null) {
                // ...

                await using var cmd = new NpgsqlCommand();
                cmd.Connection = _connection;

                cmd.Parameters.Add(
                    new NpgsqlParameter<int>(
                        nameof(IntegrationEvent.Status),
                        (int) IntegrationEventStatus.Published
                    )
                );

                for (int i = 0; i < events.Count; ++i) {
                    var @event = events[i];
                    cmd.Parameters.Add(new NpgsqlParameter<int>(
                        $"{nameof(IntegrationEvent.Id)}{i}", @event.Id
                    ));
                }

                int j = 0;
                var placeholderBuilder = new StringBuilder();
                foreach (var parameter in cmd.Parameters.Skip(1)) {
                    if (j++ == 0) {
                        placeholderBuilder.Append($"@{parameter.ParameterName}");
                    } else {
                        placeholderBuilder.Append($", @{parameter.ParameterName}");
                    }
                }

                cmd.CommandText = $@"
                    UPDATE identity.""{nameof(UserDbContext.IntegrationEvents)}""
                    SET ""{nameof(IntegrationEvent.Status)}"" =
                        @{cmd.Parameters.First().ParameterName}
                    WHERE
                        ""{nameof(IntegrationEvent.Id)}"" IN ({placeholderBuilder});
                ";

                await cmd.ExecuteNonQueryAsync();
            }

            await txn.CommitAsync();
        }

        public async Task FetchAndPublishEventById(int eventId) {
            _ensureConnection();
            await _ensureConnectionOpen();

            await using var txn =
                await _connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            IntegrationEvent @event = null;

            await using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = _connection;
                
                cmd.Parameters.Add(
                    new NpgsqlParameter<int>(nameof(IntegrationEvent.Id), eventId)
                );
                cmd.Parameters.Add(
                    new NpgsqlParameter<int>(
                        nameof(IntegrationEvent.Status),
                        (int) IntegrationEventStatus.Pending
                    )
                );

                cmd.CommandText = $@"
                    SELECT
                        ""{nameof(IntegrationEvent.Type)}"",
                        ""{nameof(IntegrationEvent.Payload)}""
                    FROM
                        identity.""{nameof(UserDbContext.IntegrationEvents)}""
                    WHERE
                        ""{nameof(IntegrationEvent.Id)}"" =
                            @{cmd.Parameters.First().ParameterName} AND
                        ""{nameof(IntegrationEvent.Status)}"" =
                            @{cmd.Parameters.Last().ParameterName}
                    FOR NO KEY UPDATE;
                ";

                await using var reader = await cmd.ExecuteReaderAsync(
                    CommandBehavior.SingleRow
                );

                if (await reader.ReadAsync()) {
                    int i = 0;
                    @event = new IntegrationEvent {
                        Id = eventId,
                        Type = (IntegrationEventType) reader.GetInt32(i),
                        Payload = reader.GetFieldValue<JsonDocument>(++i),
                        Status = IntegrationEventStatus.Pending
                    };
                }
            }

            if (@event != null) {
                // ...

                await using var cmd = new NpgsqlCommand();
                cmd.Connection = _connection;

                cmd.Parameters.Add(
                    new NpgsqlParameter<int>(nameof(IntegrationEvent.Id), eventId)
                );
                cmd.Parameters.Add(
                    new NpgsqlParameter<int>(
                        nameof(IntegrationEvent.Status),
                        (int) IntegrationEventStatus.Published
                    )
                );

                cmd.CommandText = $@"
                    UPDATE identity.""{nameof(UserDbContext.IntegrationEvents)}""
                    SET ""{nameof(IntegrationEvent.Status)}"" =
                        @{cmd.Parameters.Last().ParameterName}
                    WHERE
                        ""{nameof(IntegrationEvent.Id)}"" =
                            @{cmd.Parameters.First().ParameterName};
                ";

                await cmd.ExecuteNonQueryAsync();
            }

            await txn.CommitAsync();
        }
    }
}
