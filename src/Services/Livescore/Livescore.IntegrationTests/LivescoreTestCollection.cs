using Xunit;

namespace Livescore.IntegrationTests {
    [CollectionDefinition(nameof(LivescoreTestCollection))]
    public class LivescoreTestCollection : ICollectionFixture<Sut> { }
}
