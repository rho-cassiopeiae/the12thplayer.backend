using Xunit;

namespace Feed.IntegrationTests {
    [CollectionDefinition(nameof(FeedTestCollection))]
    public class FeedTestCollection : ICollectionFixture<Sut> { }
}
