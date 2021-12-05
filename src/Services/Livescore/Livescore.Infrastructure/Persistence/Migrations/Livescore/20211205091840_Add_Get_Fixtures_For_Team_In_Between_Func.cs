using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations.Livescore
{
    public partial class Add_Get_Fixtures_For_Team_In_Between_Func : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("Livescore.Infrastructure.Persistence.Migrations.Livescore.Scripts.Functions.GetFixturesForTeamInBetween.GetFixturesForTeamInBetween.v0.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS livescore.get_fixtures_for_team_in_between;");
        }
    }
}
