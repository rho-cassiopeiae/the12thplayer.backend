using System.Data;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Common.Interfaces;
using Feed.Application.Common.Results;
using Feed.Application.Common.Attributes;
using Feed.Domain.Aggregates.Comment;
using Feed.Domain.Aggregates.UserVote;
using Feed.Domain.Base;

namespace Feed.Application.Comment.Commands.VoteForComment {
    [RequireAuthorization]
    public class VoteForCommentCommand : IRequest<HandleResult<CommentRatingDto>> {
        public long ArticleId { get; set; }
        public string CommentId { get; set; }
        public short? UserVote { get; set; }
    }

    public class VoteForCommentCommandHandler : IRequestHandler<
        VoteForCommentCommand, HandleResult<CommentRatingDto>
    > {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserVoteRepository _userVoteRepository;
        private readonly ICommentRepository _commentRepository;

        public VoteForCommentCommandHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IUnitOfWork unitOfWork,
            IUserVoteRepository userVoteRepository,
            ICommentRepository commentRepository
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _unitOfWork = unitOfWork;
            _userVoteRepository = userVoteRepository;
            _commentRepository = commentRepository;
        }

        public async Task<HandleResult<CommentRatingDto>> Handle(
            VoteForCommentCommand command, CancellationToken cancellationToken
        ) {
            long userId = _principalDataProvider.GetId(_authenticationContext.User);

            await _unitOfWork.Begin(IsolationLevel.ReadCommitted);
            try {
                _userVoteRepository.EnlistAsPartOf(_unitOfWork);

                var userVote = await _userVoteRepository.UpdateOneAndGetOldForComment(
                    userId, command.ArticleId, command.CommentId, command.UserVote
                );

                int incrementRatingBy = userVote.ChangeCommentVote(command.CommentId, command.UserVote);

                long updatedRating = await _commentRepository.UpdateRatingFor(
                    command.ArticleId, command.CommentId, incrementRatingBy
                );

                await _unitOfWork.Commit();

                return new HandleResult<CommentRatingDto> {
                    Data = new CommentRatingDto {
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
