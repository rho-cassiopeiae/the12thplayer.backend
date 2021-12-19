using System.Linq;
using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using Feed.Application.Article.Commands.PostArticle;
using Feed.Application.Author.Commands.CreateAuthor;
using Feed.Domain.Aggregates.Article;
using Feed.Application.Comment.Commands.PostComment;
using Feed.Application.Comment.Queries.GetCommentsForArticle;

namespace Feed.IntegrationTests.Comment.Queries {
    [Collection(nameof(FeedTestCollection))]
    public class Get_Comments_For_Article_Query_Tests {
        private readonly Sut _sut;

        private readonly long _authorId;
        private readonly string _authorUsername;
        private readonly long _articleId;

        public Get_Comments_For_Article_Query_Tests(Sut sut) {
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
        }

        [Fact]
        public async Task Should_Retrieve_Top_Comments() {
            _sut.RunAs(userId: _authorId, username: _authorUsername);

            var commentId1 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = null,
                ParentCommentId = null,
                Body = "body"
            })).Data;
            var commentId11 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = commentId1,
                ParentCommentId = commentId1,
                Body = "body"
            })).Data;
            var commentId111 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = commentId1,
                ParentCommentId = commentId11,
                Body = "body"
            })).Data;
            var commentId12 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = commentId1,
                ParentCommentId = commentId1,
                Body = "body"
            })).Data;
            var commentId121 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = commentId1,
                ParentCommentId = commentId12,
                Body = "body"
            })).Data;
            var commentId1211 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = commentId1,
                ParentCommentId = commentId121,
                Body = "body"
            })).Data;
            var commentId122 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = commentId1,
                ParentCommentId = commentId12,
                Body = "body"
            })).Data;
            var commentId13 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = commentId1,
                ParentCommentId = commentId1,
                Body = "body"
            })).Data;

            var commentId2 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = null,
                ParentCommentId = null,
                Body = "body"
            })).Data;
            var commentId21 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = commentId2,
                ParentCommentId = commentId2,
                Body = "body"
            })).Data;
            var commentId211 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = commentId2,
                ParentCommentId = commentId21,
                Body = "body"
            })).Data;
            var commentId22 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = commentId2,
                ParentCommentId = commentId2,
                Body = "body"
            })).Data;

            var commentId3 = (await _sut.SendRequest(new PostCommentCommand {
                ArticleId = _articleId,
                ThreadRootCommentId = null,
                ParentCommentId = null,
                Body = "body"
            })).Data;

            var result = await _sut.SendRequest(new GetCommentsForArticleQuery {
                ArticleId = _articleId,
                Filter = (int) CommentFilter.Top,
                Page = 1
            });

            var comments = result.Data;

            comments.Should().HaveCount(13);
            comments.Where(c => c.ParentId == null).Should().HaveCount(3);
            comments.Where(c => c.RootId == commentId1).Should().HaveCount(8);
        }
    }
}
