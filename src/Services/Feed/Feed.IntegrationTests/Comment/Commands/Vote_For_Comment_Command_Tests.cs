using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using Feed.Application.Article.Commands.PostArticle;
using Feed.Application.Author.Commands.CreateAuthor;
using Feed.Application.Comment.Commands.PostComment;
using Feed.Application.Comment.Commands.VoteForComment;
using Feed.Domain.Aggregates.Article;

namespace Feed.IntegrationTests.Comment.Commands {
    [Collection(nameof(FeedTestCollection))]
    public class Vote_For_Comment_Command_Tests {
        private readonly Sut _sut;

        private readonly long _authorId;
        private readonly string _authorUsername;
        private readonly long _articleId;
        private readonly string _commentId;

        public Vote_For_Comment_Command_Tests(Sut sut) {
            _sut = sut;
            _sut.ResetState();

            _authorId = 1;
            _authorUsername = "user-1";

            _sut.SendRequest(
                new CreateAuthorCommand {
                    UserId = _authorId,
                    Email = "user@email.com",
                    Username = _authorUsername
                }
            ).Wait();

            _sut.RunAs(userId: _authorId, username: _authorUsername);

            _articleId = _sut.SendRequest(
                new PostArticleCommand {
                    TeamId = 53,
                    Type = (short) ArticleType.News,
                    Title = "title",
                    PreviewImageUrl = "previewImageUrl",
                    Summary = null,
                    Content = "content"
                }
            ).Result.Data;

            _commentId = _sut.SendRequest(
                new PostCommentCommand {
                    ArticleId = _articleId,
                    ThreadRootCommentId = null,
                    ParentCommentId = null,
                    Body = "body"
                }
            ).Result.Data;
        }

        [Fact]
        public async Task Should_Update_Comment_Rating_According_To_A_Sequence_Of_Vote_Commands() {
            _sut.RunAs(userId: _authorId, username: _authorUsername);

            await _sut.SendRequest(new VoteForCommentCommand {
                ArticleId = _articleId,
                CommentId = _commentId,
                Vote = 1 // upvote
            });

            await _sut.SendRequest(new VoteForCommentCommand {
                ArticleId = _articleId,
                CommentId = _commentId,
                Vote = 1 // revert upvote
            });

            await _sut.SendRequest(new VoteForCommentCommand {
                ArticleId = _articleId,
                CommentId = _commentId,
                Vote = -1 // downvote
            });

            await _sut.SendRequest(new VoteForCommentCommand {
                ArticleId = _articleId,
                CommentId = _commentId,
                Vote = 1 // revert downvote then upvote
            });

            await _sut.SendRequest(new VoteForCommentCommand {
                ArticleId = _articleId,
                CommentId = _commentId,
                Vote = -1 // revert upvote then downnote
            });

            var result = await _sut.SendRequest(new VoteForCommentCommand {
                ArticleId = _articleId,
                CommentId = _commentId,
                Vote = 1 // revert downvote then upvote
            });

            result.Data.Should().BeEquivalentTo(new CommentRatingDto {
                Rating = 1,
                Vote = 1
            });
        }
    }
}
