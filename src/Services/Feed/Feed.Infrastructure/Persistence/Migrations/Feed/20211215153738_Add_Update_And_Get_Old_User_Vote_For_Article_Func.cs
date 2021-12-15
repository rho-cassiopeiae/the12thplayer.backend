using Microsoft.EntityFrameworkCore.Migrations;

namespace Feed.Infrastructure.Persistence.Migrations.Feed
{
    public partial class Add_Update_And_Get_Old_User_Vote_For_Article_Func : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("Feed.Infrastructure.Persistence.Migrations.Feed.Scripts.Functions.UpdateAndGetOldUserVoteForArticle.UpdateAndGetOldUserVoteForArticle.v0.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS feed.update_and_get_old_user_vote_for_article;");
        }
    }
}
