using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.Infrastructure.Persistence.Migrations.User
{
    public partial class Add_DeviceId_To_RefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToken",
                schema: "identity",
                table: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                schema: "identity",
                table: "RefreshToken",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToken",
                schema: "identity",
                table: "RefreshToken",
                columns: new[] { "UserId", "DeviceId", "Value" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToken",
                schema: "identity",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                schema: "identity",
                table: "RefreshToken");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToken",
                schema: "identity",
                table: "RefreshToken",
                columns: new[] { "UserId", "Value" });
        }
    }
}
