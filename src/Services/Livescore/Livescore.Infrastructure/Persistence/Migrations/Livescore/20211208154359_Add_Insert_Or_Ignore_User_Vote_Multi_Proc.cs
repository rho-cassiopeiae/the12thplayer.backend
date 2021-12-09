using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations.Livescore
{
    public partial class Add_Insert_Or_Ignore_User_Vote_Multi_Proc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("Livescore.Infrastructure.Persistence.Migrations.Livescore.Scripts.StoredProcedures.InsertOrIgnoreUserVoteMulti.InsertOrIgnoreUserVoteMulti.v0.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS livescore.insert_or_ignore_user_vote_multi (bigint[], bigint[], bigint[], jsonb[], jsonb[], jsonb[]);");
        }
    }
}
