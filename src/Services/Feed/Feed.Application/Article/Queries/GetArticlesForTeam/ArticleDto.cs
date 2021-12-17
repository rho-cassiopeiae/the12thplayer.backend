namespace Feed.Application.Article.Queries.GetArticlesForTeam {
    public class ArticleDto {
        public long Id { get; init; }
        public long AuthorId { get; init; }
        public string AuthorUsername { get; init; }
        public long PostedAt { get; init; }
        public short Type { get; init; }
        public string Title { get; init; }
        public string PreviewImageUrl { get; init; }
        public string Summary { get; init; }
        public long Rating { get; init; }
        public int CommentCount { get; init; }
    }
}
