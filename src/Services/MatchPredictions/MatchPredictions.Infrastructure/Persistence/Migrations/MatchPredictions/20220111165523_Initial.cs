using MatchPredictions.Domain.Aggregates.Fixture;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MatchPredictions.Infrastructure.Persistence.Migrations.MatchPredictions
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "match_predictions");

            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "match_predictions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FlagUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leagues",
                schema: "match_predictions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    IsCup = table.Column<bool>(type: "boolean", nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                schema: "match_predictions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CountryId = table.Column<long>(type: "bigint", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "match_predictions",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Season",
                schema: "match_predictions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Season", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Season_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalSchema: "match_predictions",
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rounds",
                schema: "match_predictions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<long>(type: "bigint", nullable: true),
                    EndDate = table.Column<long>(type: "bigint", nullable: true),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rounds_Season_SeasonId",
                        column: x => x.SeasonId,
                        principalSchema: "match_predictions",
                        principalTable: "Season",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fixtures",
                schema: "match_predictions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    RoundId = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    HomeTeamId = table.Column<long>(type: "bigint", nullable: false),
                    GuestTeamId = table.Column<long>(type: "bigint", nullable: false),
                    GameTime = table.Column<GameTime>(type: "jsonb", nullable: false),
                    Score = table.Column<Score>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fixtures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fixtures_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalSchema: "match_predictions",
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fixtures_Season_SeasonId",
                        column: x => x.SeasonId,
                        principalSchema: "match_predictions",
                        principalTable: "Season",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fixtures_Teams_GuestTeamId",
                        column: x => x.GuestTeamId,
                        principalSchema: "match_predictions",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fixtures_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalSchema: "match_predictions",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_GuestTeamId",
                schema: "match_predictions",
                table: "Fixtures",
                column: "GuestTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_HomeTeamId",
                schema: "match_predictions",
                table: "Fixtures",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_RoundId",
                schema: "match_predictions",
                table: "Fixtures",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_SeasonId",
                schema: "match_predictions",
                table: "Fixtures",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_SeasonId",
                schema: "match_predictions",
                table: "Rounds",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Season_LeagueId",
                schema: "match_predictions",
                table: "Season",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CountryId",
                schema: "match_predictions",
                table: "Teams",
                column: "CountryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fixtures",
                schema: "match_predictions");

            migrationBuilder.DropTable(
                name: "Rounds",
                schema: "match_predictions");

            migrationBuilder.DropTable(
                name: "Teams",
                schema: "match_predictions");

            migrationBuilder.DropTable(
                name: "Season",
                schema: "match_predictions");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "match_predictions");

            migrationBuilder.DropTable(
                name: "Leagues",
                schema: "match_predictions");
        }
    }
}
