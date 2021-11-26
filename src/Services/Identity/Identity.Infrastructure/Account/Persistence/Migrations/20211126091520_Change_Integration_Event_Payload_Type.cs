using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.Infrastructure.Account.Persistence.Migrations
{
    public partial class Change_Integration_Event_Payload_Type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                schema: "identity",
                table: "IntegrationEvents");

            migrationBuilder.AddColumn<JsonDocument>(
                name: "Payload",
                schema: "identity",
                table: "IntegrationEvents",
                type: "jsonb",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payload",
                schema: "identity",
                table: "IntegrationEvents");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                schema: "identity",
                table: "IntegrationEvents",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
