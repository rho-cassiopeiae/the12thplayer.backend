using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using Feed.Application.Author.Commands.CreateAuthor;
using Feed.Application.Article.Commands.PostArticle;
using Feed.Domain.Aggregates.Article;
using Feed.Application.Article.Commands.VoteForArticle;

namespace Feed.IntegrationTests.Article.Commands {
    [Collection(nameof(FeedTestCollection))]
    public class Vote_For_Article_Command_Tests {
        private readonly Sut _sut;

        public Vote_For_Article_Command_Tests(Sut sut) {
            _sut = sut;
            _sut.ResetState();

            _sut.SendRequest(
                new CreateAuthorCommand {
                    UserId = 1,
                    Email = "user@email.com",
                    Username = "user-1"
                }
            ).Wait();
        }

        [Fact]
        public async Task Should_Update_Article_Rating_According_To_A_Sequence_Of_Vote_Commands() {
            var postArticleResult = await _sut.SendRequest(new PostArticleCommand {
                TeamId = 53,
                Type = (int) ArticleType.News,
                Title = "title",
                PreviewImageUrl = "previewImageUrl",
                Summary = "summary",
                Content = "content"
            });

            int articleId = postArticleResult.Data;

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
