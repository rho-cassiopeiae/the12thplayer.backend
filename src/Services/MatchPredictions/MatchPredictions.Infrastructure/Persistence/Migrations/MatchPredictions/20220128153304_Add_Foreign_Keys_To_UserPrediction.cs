using Microsoft.EntityFrameworkCore.Migrations;

namespace MatchPredictions.Infrastructure.Persistence.Migrations.MatchPredictions
{
    public partial class Add_Foreign_Keys_To_UserPrediction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserPredictions_RoundId",
                schema: "match_predictions",
                table: "UserPredictions",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPredictions_SeasonId",
                schema: "match_predictions",
                table: "UserPredictions",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPredictions_Rounds_RoundId",
                schema: "match_predictions",
                table: "UserPredictions",
                column: "RoundId",
                principalSchema: "match_predictions",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPredictions_Season_SeasonId",
                schema: "match_predictions",
                table: "UserPredictions",
                column: "SeasonId",
                principalSchema: "match_predictions",
                principalTable: "Season",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPredictions_Rounds_RoundId",
                schema: "match_predictions",
                table: "UserPredictions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPredictions_Season_SeasonId",
                schema: "match_predictions",
                table: "UserPredictions");

            migrationBuilder.DropIndex(
                name: "IX_UserPredictions_RoundId",
                schema: "match_predictions",
                table: "UserPredictions");

            migrationBuilder.DropIndex(
                name: "IX_UserPredictions_SeasonId",
                schema: "match_predictions",
                table: "UserPredictions");
        }
    }
}
