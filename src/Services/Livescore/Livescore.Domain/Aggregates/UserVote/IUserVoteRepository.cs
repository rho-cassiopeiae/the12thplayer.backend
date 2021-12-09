using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.UserVote {
    public interface IUserVoteRepository : IRepository<UserVote> {
        Task Create(IEnumerable<UserVote> userVotes);
    }
}
