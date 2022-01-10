using System.Text.Json.Serialization;

namespace Feed.Application.Comment.Queries.GetCommentsForArticle {
    public class CommentWithUserVoteDto {
        public string Id { get; init; }
        public string RootId { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ParentId { get; init; }
        public long AuthorId { get; init; }
        public string AuthorUsername { get; init; }
        public long Rating { get; init; }
        public string Body { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public short? UserVote { get; set; }
    }
}
