using System.Threading.Tasks;
using System.Collections.Generic;

using Xunit;
using FluentAssertions;

using Livescore.Application.Livescore.VideoReaction.Commands.PostVideoReaction;
using Livescore.Application.Livescore.VideoReaction.Commands.VoteForVideoReaction;
using Livescore.Application.Livescore.Worker.Commands.ActivateFixture;

namespace Livescore.IntegrationTests.Livescore.VideoReaction.Commands {
    [Collection(nameof(LivescoreTestCollection))]
    public class Vote_For_Video_Reaction_Command_Tests {
        private readonly Sut _sut;

        private readonly long _fixtureId;
        private readonly long _teamId;
        private readonly long _authorId;

        public Vote_For_Video_Reaction_Command_Tests(Sut sut) {
            _sut = sut;
            _sut.ResetState();

            (_fixtureId, _teamId) = _sut.SeedWithDummyUpcomingFixture();

            _sut.SendRequest(
                new ActivateFixtureCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    VimeoProjectId = "789456"
                }
            ).Wait();

            using var preparedRequest = _sut.PrepareHttpRequestForFileUpload(
                "test-video.mp4",
                new KeyValuePair<string, string>("title", "test-title")
            );

            _authorId = 1;
            _sut.RunAs(userId: _authorId, username: $"user-{_authorId}");

            _sut.SendRequest(
                new PostVideoReactionCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    Request = preparedRequest.Request
                }
            ).Wait();
        }

        [Fact]
        public async Task Should_Update_Video_Reaction_Rating_According_To_Last_User_Vote() {
            _sut.RunAs(userId: 2, username: "user-2");

            await Task.WhenAll(new[] {
                _sut.SendRequest(new VoteForVideoReactionCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    AuthorId = _authorId,
                    UserVote = -1
                }),
                _sut.SendRequest(new VoteForVideoReactionCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    AuthorId = _authorId,
                    UserVote = 1
                }),
                _sut.SendRequest(new VoteForVideoReactionCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    AuthorId = _authorId,
                    UserVote = null
                })
            });

            var result = await _sut.SendRequest(new VoteForVideoReactionCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                AuthorId = _authorId,
                UserVote = 1
            });

            result.Data.Should().BeEquivalentTo(new VideoReactionRatingDto {
                Rating = 2
            });
        }
    }
}
