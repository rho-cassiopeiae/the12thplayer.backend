using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Npgsql;
using NpgsqlTypes;

using Feed.Domain.Aggregates.Author;

namespace Feed.Infrastructure.Persistence.Repositories {
    public class AuthorRepository : IAuthorRepository {
        private readonly FeedDbContext _feedDbContext;

        public AuthorRepository(FeedDbContext feedDbContext) {
            _feedDbContext = feedDbContext;
        }

        public Task SaveChanges(CancellationToken cancellationToken) => Task.CompletedTask;

        public async Task Create(Author author) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter<long>(nameof(Author.UserId), NpgsqlDbType.Bigint) {
                    TypedValue = author.UserId
                },
                new NpgsqlParameter<string>(nameof(Author.Email), NpgsqlDbType.Text) {
                    TypedValue = author.Email
                },
                new NpgsqlParameter<string>(nameof(Author.Username), NpgsqlDbType.Text) {
                    TypedValue = author.Username
                }
            };

            cmd.Parameters.AddRange(parameters);

            int i = 0;
            cmd.CommandText = $@"
                INSERT INTO feed.""Authors"" (""UserId"", ""Email"", ""Username"")
                VALUES (@{parameters[i++].ParameterName}, @{parameters[i++].ParameterName}, @{parameters[i++].ParameterName});
            ";

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdatePermissions(Author author) {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = await _feedDbContext.Database.GetDbConnection();

            var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter<long>(nameof(AuthorPermission.UserId), NpgsqlDbType.Bigint) {
                    TypedValue = author.UserId
                },
                new NpgsqlParameter<int[]>(nameof(AuthorPermission.Scope), NpgsqlDbType.Array | NpgsqlDbType.Integer) {
                    TypedValue = author.Permissions.Select(p => (int) p.Scope).ToArray()
                },
                new NpgsqlParameter<int[]>(nameof(AuthorPermission.Flags), NpgsqlDbType.Array | NpgsqlDbType.Integer) {
                    TypedValue = author.Permissions.Select(p => p.Flags).ToArray()
                }
            };

            cmd.Parameters.AddRange(parameters);

            int i = 0;
            cmd.CommandText = $@"
                INSERT INTO feed.""AuthorPermissions"" (""UserId"", ""Scope"", ""Flags"")
                    SELECT @{parameters[i++].ParameterName}, vals.s, vals.f
                    FROM UNNEST (@{parameters[i++].ParameterName}, @{parameters[i++].ParameterName}) AS vals (s, f)
                ON CONFLICT (""UserId"", ""Scope"") DO
                    UPDATE
                    SET ""Flags"" = ""AuthorPermissions"".""Flags"" | EXCLUDED.""Flags"";
            ";

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
