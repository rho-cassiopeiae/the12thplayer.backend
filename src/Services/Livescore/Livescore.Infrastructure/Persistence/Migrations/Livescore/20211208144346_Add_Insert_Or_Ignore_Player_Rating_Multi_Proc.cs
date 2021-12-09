using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations.Livescore
{
    public partial class Add_Insert_Or_Ignore_Player_Rating_Multi_Proc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("Livescore.Infrastructure.Persistence.Migrations.Livescore.Scripts.StoredProcedures.InsertOrIgnorePlayerRatingMulti.InsertOrIgnorePlayerRatingMulti.v0.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS livescore.insert_or_ignore_player_rating_multi (bigint[], bigint[], text[], int[], int[]);");
        }
    }
}
