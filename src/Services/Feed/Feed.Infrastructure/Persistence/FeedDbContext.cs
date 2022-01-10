using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Npgsql;
using NpgsqlTypes;

using Feed.Infrastructure.Persistence.Migrations;

namespace Feed.Infrastructure.Persistence {
    public class FeedDbContext : IDisposable, IAsyncDisposable {
        public class DbSession : IDisposable, IAsyncDisposable {
            private readonly ILogger<DbSession> _logger;
            private readonly string _connectionString;
            private readonly string _migrationHistorySchema;
            private readonly string _migrationHistoryTable;

            private NpgsqlConnection _connection;
            private bool _connectionExternallyProvided;

            public DbSession(IConfiguration configuration, ILogger<DbSession> logger) {
                _logger = logger;
                _connectionString = configuration.GetConnectionString("Feed");
                _migrationHistorySchema = configuration["Migrations:Schema"];
                _migrationHistoryTable = configuration["Migrations:Table"];
                _connectionExternallyProvided = false;
            }

            public void Dispose() {
                if (_connection != null && !_connectionExternallyProvided) {
                    _connection.Dispose();
                }
            }

            public async ValueTask DisposeAsync() {
                if (_connection != null && !_connectionExternallyProvided) {
                    await _connection.DisposeAsync();
                }
            }

            public void SetDbConnection(DbConnection connection) {
                if (_connection != null) {
                    throw new Exception("The DbSession instance already has an underlying connection");
                }

                _connection = (NpgsqlConnection) connection;
                _connectionExternallyProvided = true;
            }

            public void UseTransaction(DbTransaction transaction) { } // @@NOTE: no-op since postgres doesn't support nested transactions

            public async ValueTask<NpgsqlConnection> GetDbConnection() {
                if (_connectionExternallyProvided) {
                    return _connection;
                }

                if (_connection == null) {
                    _connection = new NpgsqlConnection(_connectionString);
                }
                if (_connection.State != ConnectionState.Open) {
                    await _connection.OpenAsync();
                }

                return _connection;
            }

            public void Migrate() {
                if (_migrationHistorySchema == null || _migrationHistoryTable == null) {
                    throw new Exception("Unspecified migration history table schema and/or name");
                }

                using var transaction = GetDbConnection().Result.BeginTransaction(IsolationLevel.Serializable);

                bool migrationHistoryTableExists;
                using (var cmd = new NpgsqlCommand()) {
                    cmd.Connection = _connection;

                    var parameters = new NpgsqlParameter<string>[] {
                        new("table_schema", NpgsqlDbType.Text) {
                            TypedValue = _migrationHistorySchema
                        },
                        new("table_name", NpgsqlDbType.Text) {
                            TypedValue = _migrationHistoryTable
                        }
                    };

                    cmd.Parameters.AddRange(parameters);

                    int i = 0;
                    cmd.CommandText = $@"
                        SELECT 1
                        FROM information_schema.tables
                        WHERE
                            table_schema = @{parameters[i++].ParameterName} AND
                            table_name = @{parameters[i++].ParameterName};
                    ";

                    using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    migrationHistoryTableExists = reader.Read();
                }

                string lastAppliedMigrationName = null;
                if (!migrationHistoryTableExists) {
                    using (var cmd = new NpgsqlCommand()) {
                        cmd.Connection = _connection;
                        cmd.CommandText = $@"
                            CREATE SCHEMA IF NOT EXISTS {_migrationHistorySchema};
                        ";

                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new NpgsqlCommand()) {
                        cmd.Connection = _connection;
                        cmd.CommandText = $@"
                            CREATE TABLE {_migrationHistorySchema}.""{_migrationHistoryTable}"" (
                                ""Name"" TEXT PRIMARY KEY
                            );
                        ";

                        cmd.ExecuteNonQuery();
                    }
                } else {
                    using (var cmd = new NpgsqlCommand()) {
                        cmd.Connection = _connection;
                        cmd.CommandText = $@"
                            SELECT ""Name""
                            FROM {_migrationHistorySchema}.""{_migrationHistoryTable}""
                            ORDER BY ""Name"" DESC
                            LIMIT 1;
                        ";

                        using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                        if (reader.Read()) {
                            lastAppliedMigrationName = reader.GetString(0);
                        }
                    }
                }

                var migrationTypesToApply = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(Migration)))
                    .Where(t => lastAppliedMigrationName == null || t.Name.CompareTo(lastAppliedMigrationName) > 0)
                    .OrderBy(t => t.Name)
                    .ToList();

                foreach (var t in migrationTypesToApply) {
                    _logger.LogInformation("Applying migration {Migration}", t.Name);

                    var migration = (Migration) Activator.CreateInstance(t);
                    migration.Up(_connection);
                }

                if (migrationTypesToApply.Any()) {
                    using (var cmd = new NpgsqlCommand()) {
                        cmd.Connection = _connection;

                        cmd.Parameters.Add(new NpgsqlParameter<string[]>("Name", NpgsqlDbType.Array | NpgsqlDbType.Text) {
                            TypedValue = migrationTypesToApply.Select(t => t.Name).ToArray()
                        });

                        cmd.CommandText = $@"
                            INSERT INTO {_migrationHistorySchema}.""{_migrationHistoryTable}"" (""Name"")
                                SELECT *
                                FROM UNNEST (@{cmd.Parameters.First().ParameterName});
                        ";

                        cmd.ExecuteNonQuery();
                    }
                } else {
                    _logger.LogInformation("No new migrations to apply");
                }

                transaction.Commit();
            }
        }

        private readonly DbSession _dbSession;
        public DbSession Database => _dbSession;

        public FeedDbContext(IConfiguration configuration, ILoggerFactory loggerFactory) {
            _dbSession = new DbSession(configuration, loggerFactory.CreateLogger<DbSession>());
        }

        static FeedDbContext() {
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
        }

        public void Dispose() {
            _dbSession.Dispose();
        }

        public ValueTask DisposeAsync() {
            return _dbSession.DisposeAsync();
        }
    }
}
