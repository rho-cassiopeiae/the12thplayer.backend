using System.Collections.Generic;
using System.Threading.Tasks;

using Feed.Domain.Aggregates.UserVote;

namespace Feed.Application.Common.Interfaces {
    public interface IUserVoteQueryable {
        Task<IEnumerable<UserVote>> GetArticleVotesFor(long userId, IEnumerable<long> articleIds);
        Task<UserVote> GetArticleVoteFor(long userId, long articleId);
        Task<UserVote> GetCommentVotesFor(long userId, long articleId);
    }
}
