namespace Feed.Application.Comment.Commands.VoteForComment {
    public class CommentRatingDto {
        public long Rating { get; init; }
        public short? Vote { get; init; }
    }
}
