using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using MediatR;

using Feed.Application.Common.Results;
using Feed.Application.Common.Attributes;
using Feed.Application.Common.Interfaces;
using Feed.Application.Common.Errors;
using Feed.Application.Article.Common.Errors;
using Feed.Domain.Aggregates.Article;
using ArticleDm = Feed.Domain.Aggregates.Article.Article;

namespace Feed.Application.Article.Commands.PostVideoArticle {
    [RequireAuthorization]
    public class PostVideoArticleCommand : IRequest<HandleResult<long>> {
        public long TeamId { get; init; }
        public HttpRequest Request { get; init; }
    }

    public class PostVideoArticleCommandHandler : IRequestHandler<PostVideoArticleCommand, HandleResult<long>> {
        private readonly ILogger<PostVideoArticleCommandHandler> _logger;

        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IFileReceiver _fileReceiver;
        private readonly IFileHosting _fileHosting;

        private readonly IArticleRepository _articleRepository;

        public PostVideoArticleCommandHandler(
            ILogger<PostVideoArticleCommandHandler> logger,
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IFileReceiver fileReceiver,
            IFileHosting fileHosting,
            IArticleRepository articleRepository
        ) {
            _logger = logger;
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _fileReceiver = fileReceiver;
            _fileHosting = fileHosting;
            _articleRepository = articleRepository;
        }

        public async Task<HandleResult<long>> Handle(
            PostVideoArticleCommand command, CancellationToken cancellationToken
        ) {
            var outcome = await _fileReceiver.ReceiveImageAndFormValues(
                command.Request,
                maxSize: 2 * 1024 * 1024, // @@TODO: Config.
                filePrefix: "video-thumbnails"
            );
            if (outcome.IsError) {
                return new HandleResult<long> {
                    Error = outcome.Error
                };
            }

            var formCollection = outcome.Data;
            var filePath = formCollection["filePath"];

            bool valid = _validateForm(formCollection);
            if (!valid) {
                try {
                    _fileReceiver.DeleteFile(filePath);
                } catch (Exception e) {
                    _logger.LogError(e, "Error deleting user image");
                }

                return new HandleResult<long> {
                    Error = new ValidationError("Invalid input data")
                };
            }

            string thumbnailUrl;
            try {
                thumbnailUrl = await _fileHosting.UploadImage(filePath);
            } catch (Exception e) {
                _logger.LogError(e, "Error uploading user image");

                // @@TODO: Since here it's unclear what exactly caused uploading to fail (it could be just a MassTransit
                // error for all we know), we should not rely on FileHostingGateway deleting the file. Need to ensure that
                // it gets properly deleted.

                try {
                    _fileReceiver.DeleteFile(filePath);
                } catch (Exception ex) {
                    _logger.LogError(ex, "Error deleting user image");
                }

                return new HandleResult<long> {
                    Error = new ArticleError("Error posting video article")
                };
            }

            var article = new ArticleDm(
                teamId: command.TeamId,
                authorId: _principalDataProvider.GetId(_authenticationContext.User),
                authorUsername: _principalDataProvider.GetUsername(_authenticationContext.User),
                postedAt: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                type: (ArticleType) short.Parse(formCollection["type"]),
                title: formCollection["title"],
                previewImageUrl: thumbnailUrl,
                summary: formCollection.ContainsKey("summary") ? (string) formCollection["summary"] : null,
                content: formCollection["content"],
                rating: 0
            );

            long articleId = await _articleRepository.Create(article);

            return new HandleResult<long> {
                Data = articleId
            };
        }

        private bool _validateForm(FormCollection formCollection) {
            // @@TODO: Validation.
            return true;
        }
    }
}
