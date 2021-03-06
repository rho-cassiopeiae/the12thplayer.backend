using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.Infrastructure.Persistence.Migrations.IntegrationEvent
{
    public partial class Add_New_Integration_Event_Trigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("Identity.Infrastructure.Persistence.Migrations.IntegrationEvent.Scripts.Functions.NotifyAboutIntegrationEvent.NotifyAboutIntegrationEvent.v0.sql");
            migrationBuilder.SqlResource("Identity.Infrastructure.Persistence.Migrations.IntegrationEvent.Scripts.Triggers.OnNewIntegrationEvent.OnNewIntegrationEvent.v0.sql");

        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS identity.notify_about_integration_event CASCADE;");
        }
    }
}
