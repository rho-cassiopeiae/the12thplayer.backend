using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using MassTransit;

namespace MessageBus.Components.HostedServices {
    public class MassTransitBusController : IHostedService {
        private readonly IBusControl _busControl;

        public MassTransitBusController(IBusControl busControl) {
            _busControl = busControl;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            await _busControl.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) =>
            _busControl.StopAsync(cancellationToken);
    }
}
