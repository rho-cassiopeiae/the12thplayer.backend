using Livescore.Application.Common.Interfaces;

namespace Livescore.Api.Services {
    public class ConnectionIdProvider : IConnectionIdProvider {
        public string ConnectionId { get; set; }
    }
}
