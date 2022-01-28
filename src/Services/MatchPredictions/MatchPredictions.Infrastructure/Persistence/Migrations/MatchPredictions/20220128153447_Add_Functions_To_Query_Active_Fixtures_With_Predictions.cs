using Microsoft.EntityFrameworkCore.Migrations;

namespace MatchPredictions.Infrastructure.Persistence.Migrations.MatchPredictions
{
    public partial class Add_Functions_To_Query_Active_Fixtures_With_Predictions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("MatchPredictions.Infrastructure.Persistence.Migrations.MatchPredictions.Scripts.Functions.GetActiveSeasonsWithActiveRoundsForTeam.GetActiveSeasonsWithActiveRoundsForTeam.v0.sql");
            migrationBuilder.SqlResource("MatchPredictions.Infrastructure.Persistence.Migrations.MatchPredictions.Scripts.Functions.GetFixturesBySeasonAndRoundId.GetFixturesBySeasonAndRoundId.v0.sql");
            migrationBuilder.SqlResource("MatchPredictions.Infrastructure.Persistence.Migrations.MatchPredictions.Scripts.Functions.GetUserPredictionsBySeasonAndRoundId.GetUserPredictionsBySeasonAndRoundId.v0.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS match_predictions.get_active_seasons_with_active_rounds_for_team (BIGINT);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS match_predictions.get_fixtures_by_season_and_round_id (BIGINT[], BIGINT[]);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS match_predictions.get_user_predictions_by_season_and_round_id (BIGINT, BIGINT[], BIGINT[]);");
        }
    }
}
