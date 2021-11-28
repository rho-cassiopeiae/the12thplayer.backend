using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Npgsql;

using Identity.Domain.Base;

namespace Identity.Infrastructure.Persistence {
    public class UnitOfWork : IUnitOfWork {
        private readonly string _connectionString;
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        public DbConnection Connection => _connection;
        public DbTransaction Transaction => _transaction;

        public UnitOfWork(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString("Identity");
        }

        private async ValueTask _ensureConnectionOpen() {
            _connection ??= new NpgsqlConnection(_connectionString);
            if (_connection.State != ConnectionState.Open) {
                await _connection.OpenAsync();
            }
        }

        public async Task Begin() {
            if (_transaction != null) {
                throw new InvalidOperationException(
                    "Cannot start a transaction while another is still in progress"
                );
            }

            await _ensureConnectionOpen();

            _transaction = await _connection.BeginTransactionAsync(
                IsolationLevel.Serializable
            );
        }

        public async Task Commit() {
            if (_transaction == null) {
                throw new InvalidOperationException(
                    "There is no active transaction to commit"
                );
            }

            using (_transaction) {
                await _transaction.CommitAsync();
            }

            _transaction = null;
        }

        public async Task Rollback() {
            if (_transaction == null) {
                throw new InvalidOperationException(
                    "There is no active transaction to rollback"
                );
            }

            using (_transaction) {
                await _transaction.RollbackAsync();
            }

            _transaction = null;
        }

        public void Dispose() {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
