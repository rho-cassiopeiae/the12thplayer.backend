using System.Collections.Generic;

using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.UserVote {
    public class UserVote : Entity, IAggregateRoot {
        public long UserId { get; private set; }
        public long ArticleId { get; private set; }
        public short? ArticleVote { get; private set; }

        private Dictionary<string, short?> _commentIdToVote;
        public IReadOnlyDictionary<string, short?> CommentIdToVote => _commentIdToVote;

        public UserVote(long userId, long articleId, short? articleVote) {
            UserId = userId;
            ArticleId = articleId;
            ArticleVote = articleVote;
        }

        public UserVote(long userId, long articleId) {
            UserId = userId;
            ArticleId = articleId;
        }

        public void AddCommentVote(string commentId, short? vote) {
            _commentIdToVote ??= new Dictionary<string, short?>();
            _commentIdToVote[commentId] = vote;
        }

        public void SetCommentVotes(IDictionary<string, short?> commentVotes) {
            _commentIdToVote = new Dictionary<string, short?>(commentVotes);
        }

        public int ChangeArticleVote(short? userVote) {
            int incrementRatingBy = userVote.GetValueOrDefault() - ArticleVote.GetValueOrDefault();
            ArticleVote = userVote;

            return incrementRatingBy;
        }

        public int ChangeCommentVote(string commentId, short? userVote) {
            int incrementRatingBy = userVote.GetValueOrDefault() - _commentIdToVote[commentId].GetValueOrDefault();
            _commentIdToVote[commentId] = userVote;

            return incrementRatingBy;
        }
    }
}
