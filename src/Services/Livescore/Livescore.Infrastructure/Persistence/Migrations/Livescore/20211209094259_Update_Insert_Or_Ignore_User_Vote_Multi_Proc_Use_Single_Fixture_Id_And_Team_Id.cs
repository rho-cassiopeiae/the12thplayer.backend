using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations.Livescore
{
    public partial class Update_Insert_Or_Ignore_User_Vote_Multi_Proc_Use_Single_Fixture_Id_And_Team_Id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("Livescore.Infrastructure.Persistence.Migrations.Livescore.Scripts.StoredProcedures.InsertOrIgnoreUserVoteMulti.InsertOrIgnoreUserVoteMulti.v1.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS livescore.insert_or_ignore_user_vote_multi (bigint[], bigint, bigint, jsonb[], jsonb[], jsonb[]);");
        }
    }
}
