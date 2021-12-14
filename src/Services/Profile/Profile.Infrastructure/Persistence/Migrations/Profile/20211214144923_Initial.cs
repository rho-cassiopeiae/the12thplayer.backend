using Microsoft.EntityFrameworkCore.Migrations;

namespace Profile.Infrastructure.Persistence.Migrations.Profile
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "profile");

            migrationBuilder.CreateTable(
                name: "Profiles",
                schema: "profile",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Reputation = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ProfilePermission",
                schema: "profile",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    Flags = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilePermission", x => new { x.UserId, x.Scope });
                    table.ForeignKey(
                        name: "FK_ProfilePermission_Profiles_UserId",
                        column: x => x.UserId,
                        principalSchema: "profile",
                        principalTable: "Profiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfilePermission",
                schema: "profile");

            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "profile");
        }
    }
}
