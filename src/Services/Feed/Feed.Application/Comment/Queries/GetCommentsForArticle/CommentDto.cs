namespace Feed.Application.Comment.Queries.GetCommentsForArticle {
    public class CommentDto {
        public string Id { get; init; }
        public string RootId { get; init; }
        public string ParentId { get; init; }
        public long AuthorId { get; init; }
        public string AuthorUsername { get; init; }
        public long Rating { get; init; }
        public string Body { get; init; }
    }
}
