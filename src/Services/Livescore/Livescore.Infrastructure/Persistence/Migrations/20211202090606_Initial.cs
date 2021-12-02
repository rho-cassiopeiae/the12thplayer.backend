using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "livescore");

            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "livescore",
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
                name: "Teams",
                schema: "livescore",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CountryId = table.Column<long>(type: "bigint", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: false),
                    HasThe12thPlayerCommunity = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "livescore",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                schema: "livescore",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    BirthDate = table.Column<long>(type: "bigint", nullable: true),
                    CountryId = table.Column<long>(type: "bigint", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Managers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "livescore",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Managers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "livescore",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                schema: "livescore",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Venues_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "livescore",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Managers_CountryId",
                schema: "livescore",
                table: "Managers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_TeamId",
                schema: "livescore",
                table: "Managers",
                column: "TeamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CountryId",
                schema: "livescore",
                table: "Teams",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_TeamId",
                schema: "livescore",
                table: "Venues",
                column: "TeamId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Managers",
                schema: "livescore");

            migrationBuilder.DropTable(
                name: "Venues",
                schema: "livescore");

            migrationBuilder.DropTable(
                name: "Teams",
                schema: "livescore");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "livescore");
        }
    }
}
