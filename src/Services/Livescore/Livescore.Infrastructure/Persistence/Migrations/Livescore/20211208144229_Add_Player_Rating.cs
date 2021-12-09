using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations.Livescore
{
    public partial class Add_Player_Rating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerRatings",
                schema: "livescore",
                columns: table => new
                {
                    FixtureId = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: false),
                    ParticipantKey = table.Column<string>(type: "text", nullable: false),
                    TotalRating = table.Column<int>(type: "integer", nullable: false),
                    TotalVoters = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRatings", x => new { x.FixtureId, x.TeamId, x.ParticipantKey });
                    table.ForeignKey(
                        name: "FK_PlayerRatings_Fixtures_FixtureId_TeamId",
                        columns: x => new { x.FixtureId, x.TeamId },
                        principalSchema: "livescore",
                        principalTable: "Fixtures",
                        principalColumns: new[] { "Id", "TeamId" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerRatings",
                schema: "livescore");
        }
    }
}
