using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.Article {
    public class Article : Entity, IAggregateRoot {
        public int Id { get; private set; }
        public long TeamId { get; private set; }
        public long AuthorId { get; private set; }
        public string AuthorUsername { get; private set; }
        public long PostedAt { get; private set; }
        public ArticleType Type { get; private set; }
        public string Title { get; private set; }
        public string PreviewImageUrl { get; private set; }
        public string Summary { get; private set; }
        public string Content { get; private set; }
        public int Rating { get; private set; }

        public Article(
            long teamId, long authorId, string authorUsername,
            long postedAt, ArticleType type, string title, string previewImageUrl,
            string summary, string content, int rating
        ) {
            TeamId = teamId;
            AuthorId = authorId;
            AuthorUsername = authorUsername;
            PostedAt = postedAt;
            Type = type;
            Title = title;
            PreviewImageUrl = previewImageUrl;
            Summary = summary;
            Content = content;
            Rating = rating;
        }
    }
}
