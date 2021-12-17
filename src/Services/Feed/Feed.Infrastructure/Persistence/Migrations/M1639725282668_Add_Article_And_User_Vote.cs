using System;

using Npgsql;

namespace Feed.Infrastructure.Persistence.Migrations {
    internal class M1639725282668_Add_Article_And_User_Vote : Migration {
        public override void Up(NpgsqlConnection connection) {
            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS feed.""Articles"" (
                        ""Id"" BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                        ""TeamId"" BIGINT NOT NULL,
                        ""AuthorId"" BIGINT NOT NULL REFERENCES feed.""Authors"" (""UserId""),
                        ""AuthorUsername"" TEXT NOT NULL,
                        ""PostedAt"" BIGINT NOT NULL,
                        ""Type"" SMALLINT NOT NULL,
                        ""Title"" TEXT NOT NULL,
                        ""PreviewImageUrl"" TEXT,
                        ""Summary"" TEXT,
                        ""Content"" TEXT NOT NULL,
                        ""Rating"" BIGINT NOT NULL
                    );
                ";

                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    CREATE INDEX IF NOT EXISTS ""Articles_TeamId_Idx"" ON feed.""Articles"" (""TeamId"");
                ";

                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand()) {
                cmd.Connection = connection;
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS feed.""UserVotes"" (
                        ""UserId"" BIGINT REFERENCES feed.""Authors"" (""UserId""),
                        ""ArticleId"" BIGINT REFERENCES feed.""Articles"" (""Id""),
                        ""ArticleVote"" SMALLINT,
                        ""CommentIdToVote"" JSONB,
                        ""OldVote"" SMALLINT,
                        PRIMARY KEY (""UserId"", ""ArticleId"")
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
