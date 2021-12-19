using System.Threading.Tasks;

using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.Comment {
    public interface ICommentRepository : IRepository<Comment> {
        Task Create(Comment comment);
        Task<long> UpdateRatingFor(long articleId, string commentId, int incrementRatingBy);
    }
}
