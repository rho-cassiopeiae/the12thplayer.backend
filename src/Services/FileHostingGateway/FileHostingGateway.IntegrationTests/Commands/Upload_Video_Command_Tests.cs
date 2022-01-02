using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using FileHostingGateway.Application.Commands.AddFileFoldersForFixture;
using FileHostingGateway.Application.Commands.UploadVideo;

namespace FileHostingGateway.IntegrationTests.Commands {
    [Collection(nameof(FileHostingGatewayTestCollection))]
    public class Upload_Video_Command_Tests {
        private readonly Sut _sut;

        public Upload_Video_Command_Tests(Sut sut) {
            _sut = sut;
        }

        [Fact]
        public async Task Should_Upload_Video_To_Vimeo_When_Requested() {
            var vimeoProjectId = (await _sut.SendRequest(
                new AddFileFoldersForFixtureCommand {
                    FixtureId = 1,
                    TeamId = 1
                }
            )).Data;

            var result = await _sut.SendRequest(new UploadVideoCommand {
                FilePath = "DummyData/test-video.mp4",
                VimeoProjectId = vimeoProjectId
            });

            result.Data.Should().NotBeNull().And.NotBeEmpty();
        }
    }
}
