using Microsoft.EntityFrameworkCore.Migrations;

namespace MatchPredictions.Infrastructure.Persistence.Migrations.MatchPredictions
{
    public partial class Add_Functions_To_Submit_Match_Predictions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("MatchPredictions.Infrastructure.Persistence.Migrations.MatchPredictions.Scripts.Functions.LockAndGetUserPrediction.LockAndGetUserPrediction.v0.sql");
            migrationBuilder.SqlResource("MatchPredictions.Infrastructure.Persistence.Migrations.MatchPredictions.Scripts.Functions.FilterOutAlreadyStartedFixtures.FilterOutAlreadyStartedFixtures.v0.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS match_predictions.lock_and_get_user_prediction (BIGINT, BIGINT, BIGINT);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS match_predictions.filter_out_already_started_fixtures (BIGINT[]);");
        }
    }
}
