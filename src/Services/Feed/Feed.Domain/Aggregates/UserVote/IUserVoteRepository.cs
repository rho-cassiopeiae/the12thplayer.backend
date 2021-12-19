using System.Threading.Tasks;

using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.UserVote {
    public interface IUserVoteRepository : IRepository<UserVote> {
        Task<UserVote> UpdateOneAndGetOldForArticle(long userId, long articleId, short vote);
        Task<UserVote> UpdateOneAndGetOldForComment(long userId, long articleId, string commentId, short vote);
    }
}
