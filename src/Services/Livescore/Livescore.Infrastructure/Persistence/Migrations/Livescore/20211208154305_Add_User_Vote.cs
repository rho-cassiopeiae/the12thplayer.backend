using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Livescore.Infrastructure.Persistence.Migrations.Livescore
{
    public partial class Add_User_Vote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserVotes",
                schema: "livescore",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FixtureId = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: false),
                    FixtureParticipantKeyToRating = table.Column<IReadOnlyDictionary<string, Nullable<float>>>(type: "jsonb", nullable: true),
                    LiveCommentaryAuthorIdToVote = table.Column<IReadOnlyDictionary<string, Nullable<short>>>(type: "jsonb", nullable: true),
                    VideoReactionAuthorIdToVote = table.Column<IReadOnlyDictionary<string, Nullable<short>>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVotes", x => new { x.UserId, x.FixtureId, x.TeamId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVotes",
                schema: "livescore");
        }
    }
}
