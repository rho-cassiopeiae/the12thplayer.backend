using Identity.Infrastructure.Account.Persistence.Migrations.Scripts;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.Infrastructure.Account.Persistence.Migrations
{
    public partial class Create_Integration_Event_Trigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlResource("Identity.Infrastructure.Account.Persistence.Migrations.Scripts.Triggers.OnNewIntegrationEvent.OnNewIntegrationEvent.v0.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS identity.notify_about_integration_event CASCADE");
        }
    }
}
