using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations.Livescore
{
    public partial class Add_Foreign_Key_To_UserVote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserVotes_FixtureId_TeamId",
                schema: "livescore",
                table: "UserVotes",
                columns: new[] { "FixtureId", "TeamId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserVotes_Fixtures_FixtureId_TeamId",
                schema: "livescore",
                table: "UserVotes",
                columns: new[] { "FixtureId", "TeamId" },
                principalSchema: "livescore",
                principalTable: "Fixtures",
                principalColumns: new[] { "Id", "TeamId" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserVotes_Fixtures_FixtureId_TeamId",
                schema: "livescore",
                table: "UserVotes");

            migrationBuilder.DropIndex(
                name: "IX_UserVotes_FixtureId_TeamId",
                schema: "livescore",
                table: "UserVotes");
        }
    }
}
