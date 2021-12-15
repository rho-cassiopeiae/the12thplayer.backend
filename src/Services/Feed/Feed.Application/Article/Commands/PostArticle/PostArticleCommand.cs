using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Common.Results;
using Feed.Domain.Aggregates.Article;
using ArticleDm = Feed.Domain.Aggregates.Article.Article;

namespace Feed.Application.Article.Commands.PostArticle {
    public class PostArticleCommand : IRequest<HandleResult<int>> {
        public long TeamId { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string PreviewImageUrl { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
    }

    public class PostArticleCommandHandler : IRequestHandler<PostArticleCommand, HandleResult<int>> {
        private readonly IArticleRepository _articleRepository;

        public PostArticleCommandHandler(IArticleRepository articleRepository) {
            _articleRepository = articleRepository;
        }

        public async Task<HandleResult<int>> Handle(
            PostArticleCommand command, CancellationToken cancellationToken
        ) {
            var article = new ArticleDm(
                teamId: command.TeamId,
                authorId: 1,
                authorUsername: "user-1",
                postedAt: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                type: (ArticleType) command.Type,
                title: command.Title,
                previewImageUrl: command.PreviewImageUrl,
                summary: command.Summary,
                content: command.Content,
                rating: 0
            );

            _articleRepository.Create(article);

            await _articleRepository.SaveChanges(cancellationToken);

            return new HandleResult<int> {
                Data = article.Id
            };
        }
    }
}
