using Microsoft.Extensions.Hosting;
using HostBuilder = Microsoft.Extensions.Hosting.Host;

using FileHostingGateway.Application;
using FileHostingGateway.Host.Consumers.Worker;
using FileHostingGateway.Infrastructure;
using FileHostingGateway.Host.Consumers.Livescore;

namespace FileHostingGateway.Host {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            HostBuilder.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    var configuration = hostContext.Configuration;

                    services.AddApplication();
                    services.AddInfrastructure(
                        configuration,
                        busCfg => {
                            busCfg.AddConsumer<FolderRequestsConsumer>();
                            busCfg.AddConsumer<UploadRequestsConsumer>();
                        }
                    );
                });
    }
}
