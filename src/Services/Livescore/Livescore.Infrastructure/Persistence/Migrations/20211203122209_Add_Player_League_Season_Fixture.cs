using System.Collections.Generic;
using Livescore.Domain.Aggregates.Fixture;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations
{
    public partial class Add_Player_League_Season_Fixture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leagues",
                schema: "livescore",
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
                name: "Players",
                schema: "livescore",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    BirthDate = table.Column<long>(type: "bigint", nullable: true),
                    CountryId = table.Column<long>(type: "bigint", nullable: true),
                    Number = table.Column<short>(type: "smallint", nullable: true),
                    Position = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    LastLineupAt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "livescore",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Players_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "livescore",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Season",
                schema: "livescore",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Season", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Season_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalSchema: "livescore",
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fixtures",
                schema: "livescore",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint", nullable: true),
                    OpponentTeamId = table.Column<long>(type: "bigint", nullable: false),
                    HomeStatus = table.Column<bool>(type: "boolean", nullable: false),
                    StartTime = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    GameTime = table.Column<GameTime>(type: "jsonb", nullable: false),
                    Score = table.Column<Score>(type: "jsonb", nullable: false),
                    VenueId = table.Column<long>(type: "bigint", nullable: true),
                    RefereeName = table.Column<string>(type: "text", nullable: true),
                    Colors = table.Column<IEnumerable<TeamColor>>(type: "jsonb", nullable: false),
                    Lineups = table.Column<IEnumerable<TeamLineup>>(type: "jsonb", nullable: false),
                    Events = table.Column<IEnumerable<TeamMatchEvents>>(type: "jsonb", nullable: false),
                    Stats = table.Column<IEnumerable<TeamStats>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fixtures", x => new { x.Id, x.TeamId });
                    table.ForeignKey(
                        name: "FK_Fixtures_Season_SeasonId",
                        column: x => x.SeasonId,
                        principalSchema: "livescore",
                        principalTable: "Season",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fixtures_Teams_OpponentTeamId",
                        column: x => x.OpponentTeamId,
                        principalSchema: "livescore",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fixtures_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "livescore",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fixtures_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "livescore",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_OpponentTeamId",
                schema: "livescore",
                table: "Fixtures",
                column: "OpponentTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_SeasonId",
                schema: "livescore",
                table: "Fixtures",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_TeamId",
                schema: "livescore",
                table: "Fixtures",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_VenueId",
                schema: "livescore",
                table: "Fixtures",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_CountryId",
                schema: "livescore",
                table: "Players",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamId",
                schema: "livescore",
                table: "Players",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Season_LeagueId",
                schema: "livescore",
                table: "Season",
                column: "LeagueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fixtures",
                schema: "livescore");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "livescore");

            migrationBuilder.DropTable(
                name: "Season",
                schema: "livescore");

            migrationBuilder.DropTable(
                name: "Leagues",
                schema: "livescore");
        }
    }
}
