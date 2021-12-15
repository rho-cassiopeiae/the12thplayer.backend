using System.Collections.Generic;

using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.UserVote {
    public class UserVote : Entity, IAggregateRoot {
        public long UserId { get; private set; }
        public int ArticleId { get; private set; }
        public short? ArticleVote { get; private set; }

        private Dictionary<string, short?> _commentIdToVote;
        public IReadOnlyDictionary<string, short?> CommentIdToVote => _commentIdToVote;

        public UserVote(long userId, int articleId, short? articleVote) {
            UserId = userId;
            ArticleId = articleId;
            ArticleVote = articleVote;
        }

        public int ChangeArticleVote(short? vote) {
            int incrementRatingBy;
            if (ArticleVote == null) {
                incrementRatingBy = vote.Value;
            } else if (ArticleVote == 1) {
                if (vote == 1) {
                    incrementRatingBy = -1;
                    vote = null;
                } else {
                    incrementRatingBy = -2;
                }
            } else {
                if (vote == -1) {
                    incrementRatingBy = 1;
                    vote = null;
                } else {
                    incrementRatingBy = 2;
                }
            }

            ArticleVote = vote;

            return incrementRatingBy;
        }
    }
}
