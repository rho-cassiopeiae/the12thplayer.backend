using Microsoft.EntityFrameworkCore.Migrations;

namespace Feed.Infrastructure.Persistence.Migrations.Feed
{
    public partial class Add_Update_Article_Rating_Func : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.SqlResource("Feed.Infrastructure.Persistence.Migrations.Feed.Scripts.Functions.UpdateArticleRating.UpdateArticleRating.v0.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS feed.update_article_rating;");
        }
    }
}
