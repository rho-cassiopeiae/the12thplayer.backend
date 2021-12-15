using Microsoft.EntityFrameworkCore.Migrations;

namespace Feed.Infrastructure.Persistence.Migrations.Feed
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "feed");

            migrationBuilder.CreateTable(
                name: "Authors",
                schema: "feed",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AuthorPermission",
                schema: "feed",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    Flags = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorPermission", x => new { x.UserId, x.Scope });
                    table.ForeignKey(
                        name: "FK_AuthorPermission_Authors_UserId",
                        column: x => x.UserId,
                        principalSchema: "feed",
                        principalTable: "Authors",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorPermission",
                schema: "feed");

            migrationBuilder.DropTable(
                name: "Authors",
                schema: "feed");
        }
    }
}
