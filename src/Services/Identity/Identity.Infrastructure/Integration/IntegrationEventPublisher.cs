using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using Npgsql;
using MassTransit;

using MessageBus.Contracts;
using MessageBus.Contracts.Events.Identity;

using Identity.Infrastructure.Persistence;
using Identity.Application.Common.Integration;

namespace Identity.Infrastructure.Integration {
    public class IntegrationEventPublisher : IIntegrationEventPublisher {
        private readonly string _connectionString;
        private NpgsqlConnection _connection;

        private readonly IBus _bus;

        public IntegrationEventPublisher(
            IConfiguration configuration,
            IBus bus
        ) {
            _connectionString = configuration.GetConnectionString("Identity");
            _bus = bus;
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
                        identity.""{nameof(IntegrationEventDbContext.IntegrationEvents)}""
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
                        Id = reader.GetGuid(i),
                        Type = (IntegrationEventType) reader.GetInt32(++i),
                        Payload = reader.GetFieldValue<JsonDocument>(++i),
                        Status = IntegrationEventStatus.Pending
                    });
                }
            }

            if (events != null) {
                await _publishEvents(events);

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
                    cmd.Parameters.Add(new NpgsqlParameter<Guid>(
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
                    UPDATE identity.""{nameof(IntegrationEventDbContext.IntegrationEvents)}""
                    SET ""{nameof(IntegrationEvent.Status)}"" =
                        @{cmd.Parameters.First().ParameterName}
                    WHERE
                        ""{nameof(IntegrationEvent.Id)}"" IN ({placeholderBuilder});
                ";

                await cmd.ExecuteNonQueryAsync();
            }

            await txn.CommitAsync();
        }

        public async Task FetchAndPublishEventById(Guid eventId) {
            _ensureConnection();
            await _ensureConnectionOpen();

            await using var txn =
                await _connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            IntegrationEvent @event = null;

            await using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = _connection;
                
                cmd.Parameters.Add(
                    new NpgsqlParameter<Guid>(nameof(IntegrationEvent.Id), eventId)
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
                        identity.""{nameof(IntegrationEventDbContext.IntegrationEvents)}""
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
                await _publishEvent(@event);

                await using var cmd = new NpgsqlCommand();
                cmd.Connection = _connection;

                cmd.Parameters.Add(
                    new NpgsqlParameter<Guid>(nameof(IntegrationEvent.Id), eventId)
                );
                cmd.Parameters.Add(
                    new NpgsqlParameter<int>(
                        nameof(IntegrationEvent.Status),
                        (int) IntegrationEventStatus.Published
                    )
                );

                cmd.CommandText = $@"
                    UPDATE identity.""{nameof(IntegrationEventDbContext.IntegrationEvents)}""
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

        private T _convertIntegrationEventTo<T>(IntegrationEvent @event)
            where T : Message =>
                Message.FromJson<T>(@event.Payload, @event.Id);

        private async Task _publishEvent(IntegrationEvent @event) {
            switch (@event.Type) {
                case IntegrationEventType.UserAccountCreated:
                    await _bus.Publish(
                        _convertIntegrationEventTo<UserAccountCreated>(@event)
                    );
                    break;
                case IntegrationEventType.UserAccountConfirmed:
                    await _bus.Publish(
                        _convertIntegrationEventTo<UserAccountConfirmed>(@event)
                    );
                    break;
            }
        }

        private async Task _publishEvents(List<IntegrationEvent> events) {
            foreach (var group in events.GroupBy(@event => @event.Type)) {
                switch (group.Key) {
                    case IntegrationEventType.UserAccountCreated:
                        await _bus.PublishBatch(
                            group.Select(@event =>
                                _convertIntegrationEventTo<UserAccountCreated>(@event)
                            )
                        );
                        break;
                    case IntegrationEventType.UserAccountConfirmed:
                        await _bus.PublishBatch(
                            group.Select(@event =>
                                _convertIntegrationEventTo<UserAccountConfirmed>(@event)
                            )
                        );
                        break;
                }
            }
        }
    }
}
