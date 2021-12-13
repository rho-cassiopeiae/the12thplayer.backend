using System.Threading.Tasks;
using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

using Livescore.Application.Livescore.VideoReaction.Commands.PostVideoReaction;
using Livescore.Application.Livescore.VideoReaction.Commands.VoteForVideoReaction;
using Livescore.Application.Livescore.Worker.Commands.ActivateFixture;

namespace Livescore.IntegrationTests.Livescore.VideoReaction.Commands {
    [Collection(nameof(LivescoreTestCollection))]
    public class Vote_For_Video_Reaction_Command_Tests {
        private readonly ITestOutputHelper _output;
        private readonly Sut _sut;

        private readonly long _fixtureId;
        private readonly long _teamId;
        private readonly long _authorId;

        public Vote_For_Video_Reaction_Command_Tests(ITestOutputHelper output, Sut sut) {
            _output = output;

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
        public async Task Should_Increase_Video_Reaction_Rating_By_1_When_Upvote_For_The_First_Time() {
            _sut.RunAs(userId: 2, username: "user-2");

            var result = await _sut.SendRequest(new VoteForVideoReactionCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                AuthorId = _authorId,
                Vote = 1
            });

            result.Data.Should().BeEquivalentTo(new VideoReactionRatingDto {
                Rating = 2,
                Vote = 1
            });
        }

        [Fact]
        public async Task Should_Reset_Video_Reaction_Rating_And_User_Vote_When_Upvote_Twice() {
            _sut.RunAs(userId: 2, username: "user-2");

#pragma warning disable CS4014
            _sut.SendRequest(new VoteForVideoReactionCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                AuthorId = _authorId,
                Vote = 1
            });
#pragma warning restore

            var result = await _sut.SendRequest(new VoteForVideoReactionCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                AuthorId = _authorId,
                Vote = 1
            });

            result.Data.Should().BeEquivalentTo(new VideoReactionRatingDto {
                Rating = 1,
                Vote = null
            });
        }
    }
}
