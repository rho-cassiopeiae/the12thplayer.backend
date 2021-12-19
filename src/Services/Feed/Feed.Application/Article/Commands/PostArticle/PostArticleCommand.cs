﻿using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Common.Results;
using Feed.Application.Common.Interfaces;
using Feed.Application.Common.Attributes;
using Feed.Domain.Aggregates.Article;
using ArticleDm = Feed.Domain.Aggregates.Article.Article;

namespace Feed.Application.Article.Commands.PostArticle {
    [RequireAuthorization]
    public class PostArticleCommand : IRequest<HandleResult<long>> {
        public long TeamId { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string PreviewImageUrl { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
    }

    public class PostArticleCommandHandler : IRequestHandler<PostArticleCommand, HandleResult<long>> {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IArticleRepository _articleRepository;

        public PostArticleCommandHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IArticleRepository articleRepository
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _articleRepository = articleRepository;
        }

        public async Task<HandleResult<long>> Handle(
            PostArticleCommand command, CancellationToken cancellationToken
        ) {
            var article = new ArticleDm(
                teamId: command.TeamId,
                authorId: _principalDataProvider.GetId(_authenticationContext.User),
                authorUsername: _principalDataProvider.GetUsername(_authenticationContext.User),
                postedAt: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                type: (ArticleType) command.Type,
                title: command.Title,
                previewImageUrl: command.PreviewImageUrl,
                summary: command.Summary,
                content: command.Content,
                rating: 0
            );

            long articleId = await _articleRepository.Create(article);

            return new HandleResult<long> {
                Data = articleId
            };
        }
    }
}
