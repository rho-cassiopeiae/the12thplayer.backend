using System.Collections.Generic;
using System.Threading.Tasks;

using Feed.Application.Comment.Queries.GetCommentsForArticle;

namespace Feed.Application.Common.Interfaces {
    public interface ICommentQueryable {
        Task<IEnumerable<CommentWithUserVoteDto>> GetCommentsFor(long articleId);
    }
}
