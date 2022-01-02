using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations.Livescore
{
    public partial class Make_Season_Name_Not_Required_Add_DisplayName_To_Player_Make_Fixture_StartTime_Required : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "livescore",
                table: "Season",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "livescore",
                table: "Players",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "StartTime",
                schema: "livescore",
                table: "Fixtures",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "livescore",
                table: "Players");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "livescore",
                table: "Season",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "StartTime",
                schema: "livescore",
                table: "Fixtures",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
