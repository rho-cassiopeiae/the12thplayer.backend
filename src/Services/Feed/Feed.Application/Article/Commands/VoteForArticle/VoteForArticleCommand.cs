using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Feed.Application.Common.Results;
using Feed.Domain.Aggregates.Article;
using Feed.Domain.Aggregates.UserVote;
using Feed.Domain.Base;

using MediatR;

namespace Feed.Application.Article.Commands.VoteForArticle {
    public class VoteForArticleCommand : IRequest<HandleResult<ArticleRatingDto>> {
        public long ArticleId { get; set; }
        public short Vote { get; set; }
    }

    public class VoteForArticleCommandHandler : IRequestHandler<
        VoteForArticleCommand, HandleResult<ArticleRatingDto>
    > {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserVoteRepository _userVoteRepository;
        private readonly IArticleRepository _articleRepository;

        public VoteForArticleCommandHandler(
            IUnitOfWork unitOfWork,
            IUserVoteRepository userVoteRepository,
            IArticleRepository articleRepository
        ) {
            _unitOfWork = unitOfWork;
            _userVoteRepository = userVoteRepository;
            _articleRepository = articleRepository;
        }

        public async Task<HandleResult<ArticleRatingDto>> Handle(
            VoteForArticleCommand command, CancellationToken cancellationToken
        ) {
            long userId = 1;

            await _unitOfWork.Begin(IsolationLevel.ReadCommitted);
            try {
                _userVoteRepository.EnlistAsPartOf(_unitOfWork);

                var userVote = await _userVoteRepository.UpdateOneAndGetOldForArticle(
                    userId, command.ArticleId, command.Vote
                );

                int incrementRatingBy = userVote.ChangeArticleVote(command.Vote);

                long updatedRating = await _articleRepository.UpdateRatingFor(
                    command.ArticleId, incrementRatingBy
                );

                await _unitOfWork.Commit();

                return new HandleResult<ArticleRatingDto> {
                    Data = new ArticleRatingDto {
                        Rating = updatedRating,
                        Vote = userVote.ArticleVote
                    }
                };
            } catch {
                await _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
