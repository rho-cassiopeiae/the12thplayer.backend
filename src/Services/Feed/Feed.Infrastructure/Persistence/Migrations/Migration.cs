using Npgsql;

namespace Feed.Infrastructure.Persistence.Migrations {
    internal abstract class Migration {
        public abstract void Up(NpgsqlConnection connection);
        public abstract void Down(NpgsqlConnection connection);
    }
}
