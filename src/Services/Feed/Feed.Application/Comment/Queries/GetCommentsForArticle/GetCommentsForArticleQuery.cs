using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Common.Interfaces;
using Feed.Application.Common.Results;

namespace Feed.Application.Comment.Queries.GetCommentsForArticle {
    public class GetCommentsForArticleQuery : IRequest<HandleResult<IEnumerable<CommentDto>>> {
        public long ArticleId { get; set; }
        public int Filter { get; set; }
        public int Page { get; set; }
    }

    public class GetCommentsForArticleQueryHandler : IRequestHandler<
        GetCommentsForArticleQuery, HandleResult<IEnumerable<CommentDto>>
    > {
        private readonly ICommentQueryable _commentQueryable;

        public GetCommentsForArticleQueryHandler(ICommentQueryable commentQueryable) {
            _commentQueryable = commentQueryable;
        }

        public async Task<HandleResult<IEnumerable<CommentDto>>> Handle(
            GetCommentsForArticleQuery query, CancellationToken cancellationToken
        ) {
            var comments = await _commentQueryable.GetCommentsFor(
                query.ArticleId, (CommentFilter) query.Filter, query.Page
            );

            return new HandleResult<IEnumerable<CommentDto>> {
                Data = comments.Select(c => new CommentDto {
                    Id = c.Id,
                    RootId = c.RootId,
                    ParentId = c.ParentId,
                    AuthorId = c.AuthorId,
                    AuthorUsername = c.AuthorUsername,
                    Rating = c.Rating,
                    Body = c.Body
                })
            };
        }
    }
}
