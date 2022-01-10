using System.Text.Json.Serialization;

namespace Feed.Application.Article.Queries.Common.Dto {
    public class ArticleWithUserVoteDto {
        public long Id { get; init; }
        public long AuthorId { get; init; }
        public string AuthorUsername { get; init; }
        public long PostedAt { get; init; }
        public short Type { get; init; }
        public string Title { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PreviewImageUrl { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Summary { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Content { get; init; }
        public long Rating { get; init; }
        public int CommentCount { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public short? UserVote { get; set; }
    }
}
