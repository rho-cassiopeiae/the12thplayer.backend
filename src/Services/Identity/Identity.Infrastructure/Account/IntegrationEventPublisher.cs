using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Npgsql;

using Identity.Infrastructure.Account.Persistence;
using Identity.Infrastructure.Account.Persistence.Models;

namespace Identity.Infrastructure.Account {
    public class IntegrationEventPublisher : IIntegrationEventPublisher {
        private readonly string _connectionString;
        private NpgsqlConnection _connection;

        public IntegrationEventPublisher(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString("Identity"); ;
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
                    @event = new IntegrationEvent {
                        Id = eventId,
                        Type = (IntegrationEventType) reader.GetInt32(0),
                        Payload = reader.GetFieldValue<JsonDocument>(1),
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
