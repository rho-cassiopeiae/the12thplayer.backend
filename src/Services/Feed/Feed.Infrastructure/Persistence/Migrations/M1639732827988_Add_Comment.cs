using System;

using Npgsql;

namespace Feed.Infrastructure.Persistence.Migrations {
    internal class M1639732827988_Add_Comment : Migration {
        public override void Up(NpgsqlConnection connection) {
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = connection;
                cmd.CommandText = $@"
                    CREATE TABLE IF NOT EXISTS feed.""Comments"" (
                        ""ArticleId"" BIGINT,
                        ""Id"" TEXT,
                        ""RootId"" TEXT NOT NULL,
                        ""ParentId"" TEXT,
                        ""AuthorId"" BIGINT NOT NULL REFERENCES feed.""Authors"" (""UserId""),
                        ""AuthorUsername"" TEXT NOT NULL,
                        ""Rating"" BIGINT NOT NULL,
                        ""Body"" TEXT NOT NULL,
                        PRIMARY KEY (""ArticleId"", ""Id"")
                    );
                ";

                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = connection;
                cmd.CommandText = $@"
                    CREATE INDEX IF NOT EXISTS ""Comments_ArticleId_RootId_Idx"" ON feed.""Comments"" (""ArticleId"", ""RootId"");
                ";

                cmd.ExecuteNonQuery();
            }
        }

        public override void Down(NpgsqlConnection connection) {
            throw new NotImplementedException();
        }
    }
}
