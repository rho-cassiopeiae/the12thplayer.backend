using Xunit;

namespace Identity.IntegrationTests {
    [CollectionDefinition(nameof(IdentityTestCollection))]
    public class IdentityTestCollection : ICollectionFixture<Sut> { }
}
