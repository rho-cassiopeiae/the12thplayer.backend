using Livescore.Infrastructure.FileUpload;

namespace Livescore.IntegrationTests.Livescore.VideoReaction.Mocks {
    internal class RandomFileNameProviderMock : IRandomFileNameProvider {
        public static readonly string Name = "random-file";
        
        public string Get() => Name;
    }
}
