using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations.Livescore
{
    public partial class Add_UDFs_For_Team_Related_Queries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("Livescore.Infrastructure.Persistence.Migrations.Livescore.Scripts.Functions.GetPlayersWithCountryFrom.GetPlayersWithCountryFrom.v0.sql");
            migrationBuilder.SqlResource("Livescore.Infrastructure.Persistence.Migrations.Livescore.Scripts.Functions.GetManagerWithCountryFrom.GetManagerWithCountryFrom.v0.sql");
            migrationBuilder.SqlResource("Livescore.Infrastructure.Persistence.Migrations.Livescore.Scripts.Functions.GetPlayerRatingsForParticipant.GetPlayerRatingsForParticipant.v0.sql");

        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS livescore.get_players_with_country_from (BIGINT);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS livescore.get_manager_with_country_from (BIGINT);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS livescore.get_player_ratings_for_participant (BIGINT, TEXT[]);");

        }
    }
}
