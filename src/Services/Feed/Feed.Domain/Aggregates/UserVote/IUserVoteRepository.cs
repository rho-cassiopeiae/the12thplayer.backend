using System.Threading.Tasks;

using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.UserVote {
    public interface IUserVoteRepository : IRepository<UserVote> {
        Task<UserVote> UpdateOneAndGetOldForArticle(long userId, int articleId, short vote);
    }
}
