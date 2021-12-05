using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations.Livescore
{
    public partial class Update_Get_Fixtures_For_Team_In_Between_Func_Specify_Schema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("Livescore.Infrastructure.Persistence.Migrations.Livescore.Scripts.Functions.GetFixturesForTeamInBetween.GetFixturesForTeamInBetween.v1.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("Livescore.Infrastructure.Persistence.Migrations.Livescore.Scripts.Functions.GetFixturesForTeamInBetween.GetFixturesForTeamInBetween.v0.sql");
        }
    }
}
