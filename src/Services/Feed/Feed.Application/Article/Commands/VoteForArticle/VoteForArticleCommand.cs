using System.Data;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Common.Attributes;
using Feed.Application.Common.Interfaces;
using Feed.Application.Common.Results;
using Feed.Domain.Aggregates.Article;
using Feed.Domain.Aggregates.UserVote;
using Feed.Domain.Base;

namespace Feed.Application.Article.Commands.VoteForArticle {
    [RequireAuthorization]
    public class VoteForArticleCommand : IRequest<HandleResult<ArticleRatingDto>> {
        public long ArticleId { get; set; }
        public short? UserVote { get; set; }
    }

    public class VoteForArticleCommandHandler : IRequestHandler<
        VoteForArticleCommand, HandleResult<ArticleRatingDto>
    > {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserVoteRepository _userVoteRepository;
        private readonly IArticleRepository _articleRepository;

        public VoteForArticleCommandHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IUnitOfWork unitOfWork,
            IUserVoteRepository userVoteRepository,
            IArticleRepository articleRepository
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _unitOfWork = unitOfWork;
            _userVoteRepository = userVoteRepository;
            _articleRepository = articleRepository;
        }

        public async Task<HandleResult<ArticleRatingDto>> Handle(
            VoteForArticleCommand command, CancellationToken cancellationToken
        ) {
            long userId = _principalDataProvider.GetId(_authenticationContext.User);

            await _unitOfWork.Begin(IsolationLevel.ReadCommitted);
            try {
                _userVoteRepository.EnlistAsPartOf(_unitOfWork);

                var userVote = await _userVoteRepository.UpdateOneAndGetOldForArticle(
                    userId, command.ArticleId, command.UserVote
                );

                int incrementRatingBy = userVote.ChangeArticleVote(command.UserVote);

                long updatedRating = await _articleRepository.UpdateRatingFor(
                    command.ArticleId, incrementRatingBy
                );

                await _unitOfWork.Commit();

                return new HandleResult<ArticleRatingDto> {
                    Data = new ArticleRatingDto {
                        Rating = updatedRating
                    }
                };
            } catch {
                await _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
