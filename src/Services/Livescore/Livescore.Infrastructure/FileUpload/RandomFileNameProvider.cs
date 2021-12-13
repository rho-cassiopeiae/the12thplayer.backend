using System.IO;

namespace Livescore.Infrastructure.FileUpload {
    internal class RandomFileNameProvider : IRandomFileNameProvider {
        public string Get() => Path.GetRandomFileName();
    }
}
