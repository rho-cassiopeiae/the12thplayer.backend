using System.Collections.Generic;
using System.Threading.Tasks;

using Feed.Application.Comment.Queries.GetCommentsForArticle;
using CommentDm = Feed.Domain.Aggregates.Comment.Comment;

namespace Feed.Application.Common.Interfaces {
    public interface ICommentQueryable {
        Task<IEnumerable<CommentDm>> GetCommentsFor(long articleId, CommentFilter filter, int page);
    }
}
