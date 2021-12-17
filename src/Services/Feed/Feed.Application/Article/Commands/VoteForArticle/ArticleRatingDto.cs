namespace Feed.Application.Article.Commands.VoteForArticle {
    public class ArticleRatingDto {
        public long Rating { get; init; }
        public short? Vote { get; init; }
    }
}
