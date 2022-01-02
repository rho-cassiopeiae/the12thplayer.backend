using Xunit;

namespace FileHostingGateway.IntegrationTests {
    [CollectionDefinition(nameof(FileHostingGatewayTestCollection))]
    public class FileHostingGatewayTestCollection : ICollectionFixture<Sut> { }
}
