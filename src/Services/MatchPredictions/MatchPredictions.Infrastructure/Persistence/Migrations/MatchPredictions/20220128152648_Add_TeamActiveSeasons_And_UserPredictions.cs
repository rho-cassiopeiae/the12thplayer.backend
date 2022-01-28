using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MatchPredictions.Infrastructure.Persistence.Migrations.MatchPredictions
{
    public partial class Add_TeamActiveSeasons_And_UserPredictions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamActiveSeasons",
                schema: "match_predictions",
                columns: table => new
                {
                    TeamId = table.Column<long>(type: "bigint", nullable: false),
                    ActiveSeasons = table.Column<List<long>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamActiveSeasons", x => x.TeamId);
                    table.ForeignKey(
                        name: "FK_TeamActiveSeasons_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "match_predictions",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPredictions",
                schema: "match_predictions",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    RoundId = table.Column<long>(type: "bigint", nullable: false),
                    FixtureIdToScore = table.Column<IReadOnlyDictionary<string, string>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPredictions", x => new { x.UserId, x.SeasonId, x.RoundId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamActiveSeasons",
                schema: "match_predictions");

            migrationBuilder.DropTable(
                name: "UserPredictions",
                schema: "match_predictions");
        }
    }
}
