using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using FileHostingGateway.Application.Commands.AddFileFoldersForFixture;

namespace FileHostingGateway.IntegrationTests.Commands {
    [Collection(nameof(FileHostingGatewayTestCollection))]
    public class Add_File_Folders_For_Fixture_Command_Tests {
        private readonly Sut _sut;

        public Add_File_Folders_For_Fixture_Command_Tests(Sut sut) {
            _sut = sut;
        }

        [Fact]
        public async Task Should_Create_Vimeo_Project_When_Requested() {
            var result = await _sut.SendRequest(new AddFileFoldersForFixtureCommand {
                FixtureId = 1,
                TeamId = 1
            });

            result.Data.Should().NotBeNull().And.NotBeEmpty();
        }
    }
}
