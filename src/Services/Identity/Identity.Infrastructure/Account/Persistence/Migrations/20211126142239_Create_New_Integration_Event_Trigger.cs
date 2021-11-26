using Microsoft.EntityFrameworkCore.Migrations;

using Identity.Infrastructure.Account.Persistence.Migrations.Scripts;

namespace Identity.Infrastructure.Account.Persistence.Migrations
{
    public partial class Create_New_Integration_Event_Trigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("Identity.Infrastructure.Account.Persistence.Migrations.Scripts.Functions.NotifyAboutIntegrationEvent.NotifyAboutIntegrationEvent.v0.sql");
            migrationBuilder.SqlResource("Identity.Infrastructure.Account.Persistence.Migrations.Scripts.Triggers.OnNewIntegrationEvent.OnNewIntegrationEvent.v0.sql");

        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS identity.notify_about_integration_event CASCADE;");
        }
    }
}
