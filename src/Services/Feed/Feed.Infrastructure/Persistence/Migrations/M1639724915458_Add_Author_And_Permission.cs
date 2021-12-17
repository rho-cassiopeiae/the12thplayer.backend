using System;

using Npgsql;

namespace Feed.Infrastructure.Persistence.Migrations {
    internal class M1639724915458_Add_Author_And_Permission : Migration {
        public override void Up(NpgsqlConnection connection) {
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS feed.""Authors"" (
                        ""UserId"" BIGINT PRIMARY KEY,
                        ""Email"" TEXT NOT NULL,
                        ""Username"" TEXT NOT NULL
                    );
                ";

                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS feed.""AuthorPermissions"" (
                        ""UserId"" BIGINT,
                        ""Scope"" INTEGER,
                        ""Flags"" INTEGER NOT NULL,
                        PRIMARY KEY (""UserId"", ""Scope"")
                    );
                ";

                cmd.ExecuteNonQuery();
            }
        }

        public override void Down(NpgsqlConnection connection) {
            throw new NotImplementedException();
        }
    }
}
