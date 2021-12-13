using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xunit;
using FluentAssertions;

using Livescore.Application.Livescore.Worker.Commands.ActivateFixture;
using Livescore.Application.Livescore.VideoReaction.Commands.PostVideoReaction;
using Livescore.IntegrationTests.Livescore.VideoReaction.Mocks;

namespace Livescore.IntegrationTests.Livescore.VideoReaction.Commands {
    [Collection(nameof(LivescoreTestCollection))]
    public class Post_Video_Reaction_Command_Tests {
        private readonly Sut _sut;

        private readonly long _fixtureId;
        private readonly long _teamId;

        public Post_Video_Reaction_Command_Tests(Sut sut) {
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
        }

        [Fact]
        public async Task Should_Save_Video_On_Disk_When_Passes_Validation_Checks() {
            using var preparedRequest = _sut.PrepareHttpRequestForFileUpload(
                "test-video.mp4",
                new KeyValuePair<string, string>("title", "test-title")
            );

            _sut.RunAs(userId: 1, username: "user-1");

            var result = await _sut.SendRequest(new PostVideoReactionCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                Request = preparedRequest.Request
            });

            result.Data.Should().BeEquivalentTo(new VideoReactionStreamingInfoDto {
                VideoId = FileHostingMock.VideoId,
                ThumbnailUrl = FileHostingMock.ThumbnailUrl
            });

            var dirPath = Path.Combine(
                _sut.GetConfigurationValue<string>("UserFiles:Path"),
                $"video-reactions/f-{_fixtureId}-t-{_teamId}"
            );
            var dirInfo = new DirectoryInfo(dirPath);

            dirInfo.Exists.Should().BeTrue();
            dirInfo.GetFileSystemInfos("*.mp4").Should().HaveCount(1);
        }
    }
}
