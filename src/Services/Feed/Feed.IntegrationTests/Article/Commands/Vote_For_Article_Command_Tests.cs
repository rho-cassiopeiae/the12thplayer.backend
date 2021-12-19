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
                            Scope = (short) PermissionScope.AdminPanel,
                            Flags = (short) AdminPanelPermissions.LogIn
                        },
                        new AuthorPermissionDto {
                            Scope = (short) PermissionScope.Article,
                            Flags = (short) (
                                ArticlePermissions.Publish |
                                ArticlePermissions.Delete
                            )
                        },
                        new AuthorPermissionDto {
                            Scope = (short) PermissionScope.Article,
                            Flags = (short) (
                                ArticlePermissions.Publish |
                                ArticlePermissions.Review
                            )
                        },
                    }
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
                                ArticlePermissions.Edit
                            )
                        }
                    }
                }
            ).Wait();
        }

        [Fact]
        public async Task Should_Update_Article_Rating_According_To_A_Sequence_Of_Vote_Commands() {
            _sut.RunAs(userId: _authorId, username: _authorUsername);

            var postArticleResult = await _sut.SendRequest(new PostArticleCommand {
                TeamId = 53,
                Type = (short) ArticleType.News,
                Title = "title",
                PreviewImageUrl = "previewImageUrl",
                Summary = null,
                Content = "content"
            });

            long articleId = postArticleResult.Data;

            await _sut.SendRequest(new VoteForArticleCommand {
                ArticleId = articleId,
                Vote = 1 // upvote
            });

            await _sut.SendRequest(new VoteForArticleCommand {
                ArticleId = articleId,
                Vote = 1 // revert upvote
            });

            await _sut.SendRequest(new VoteForArticleCommand {
                ArticleId = articleId,
                Vote = -1 // downvote
            });

            await _sut.SendRequest(new VoteForArticleCommand {
                ArticleId = articleId,
                Vote = 1 // revert downvote then upvote
            });

            await _sut.SendRequest(new VoteForArticleCommand {
                ArticleId = articleId,
                Vote = -1 // revert upvote then downnote
            });

            var result = await _sut.SendRequest(new VoteForArticleCommand {
                ArticleId = articleId,
                Vote = 1 // revert downvote then upvote
            });

            result.Data.Should().BeEquivalentTo(new ArticleRatingDto {
                Rating = 1,
                Vote = 1
            });
        }
    }
}
