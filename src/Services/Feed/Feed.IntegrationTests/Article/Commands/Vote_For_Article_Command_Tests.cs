using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using Feed.Application.Author.Commands.CreateAuthor;
using Feed.Application.Article.Commands.PostArticle;
using Feed.Domain.Aggregates.Article;
using Feed.Application.Article.Commands.VoteForArticle;
using Feed.Application.Author.Commands.AddPermissions;
using Feed.Application.Author.Common.Dto;
using Feed.Domain.Aggregates.Author;

namespace Feed.IntegrationTests.Article.Commands {
    [Collection(nameof(FeedTestCollection))]
    public class Vote_For_Article_Command_Tests {
        private readonly Sut _sut;

        private readonly long _authorId;
        private readonly string _authorUsername;

        public Vote_For_Article_Command_Tests(Sut sut) {
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

            _sut.SendRequest(
                new AddPermissionsCommand {
                    UserId = _authorId,
                    Permissions = new[] {
                        new AuthorPermissionDto {
                            Scope = (short) PermissionScope.Article,
                            Flags = (short) (
                                ArticlePermissions.Publish |
                                ArticlePermissions.Review |
                                ArticlePermissions.Edit |
                                ArticlePermissions.Delete
                            )
                        }
                    }
                }
            ).Wait();
        }

        [Fact]
        public async Task Should_Update_Article_Rating_According_To_Last_User_Vote() {
            _sut.RunAs(userId: _authorId, username: _authorUsername);

            var postArticleResult = await _sut.SendRequest(new PostArticleCommand {
                TeamId = 53,
                Type = ArticleType.News,
                Title = "title",
                PreviewImageUrl = "http://preview-image-url.com",
                Summary = null,
                Content = "content"
            });

            long articleId = postArticleResult.Data;

            await Task.WhenAll(new[] {
                _sut.SendRequest(new VoteForArticleCommand {
                    ArticleId = articleId,
                    UserVote = 1
                }),
                _sut.SendRequest(new VoteForArticleCommand {
                    ArticleId = articleId,
                    UserVote = -1
                }),
                _sut.SendRequest(new VoteForArticleCommand {
                    ArticleId = articleId,
                    UserVote = null
                }),
                _sut.SendRequest(new VoteForArticleCommand {
                    ArticleId = articleId,
                    UserVote = 1
                })
            });

            var result = await _sut.SendRequest(new VoteForArticleCommand {
                ArticleId = articleId,
                UserVote = -1
            });

            result.Data.Should().BeEquivalentTo(new ArticleRatingDto {
                Rating = -1
            });
        }
    }
}
