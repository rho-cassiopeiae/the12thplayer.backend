using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.Comment {
    public class Comment : Entity, IAggregateRoot {
        public long ArticleId { get; private set; }
        public string Id { get; private set; }
        public string RootId { get; private set; }
        public string ParentId { get; private set; }
        public long AuthorId { get; private set; }
        public string AuthorUsername { get; private set; }
        public long Rating { get; private set; }
        public string Body { get; private set; }

        public Comment(
            long articleId, string id, string rootId, string parentId,
            long authorId, string authorUsername, long rating, string body
        ) {
            ArticleId = articleId;
            Id = id;
            RootId = rootId;
            ParentId = parentId;
            AuthorId = authorId;
            AuthorUsername = authorUsername;
            Rating = rating;
            Body = body;
        }
    }
}
