using Microsoft.Extensions.Hosting;
using HostBuilder = Microsoft.Extensions.Hosting.Host;

using FileHostingGateway.Application;
using FileHostingGateway.Host.Consumers.Worker;
using FileHostingGateway.Infrastructure;
using FileHostingGateway.Host.Consumers;

namespace FileHostingGateway.Host {
    public class Program {
        public static void Main(string[] args) {
            ConfigureServices(CreateHostBuilder(args)).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => HostBuilder.CreateDefaultBuilder(args);

        public static IHostBuilder ConfigureServices(IHostBuilder hostBuilder) => hostBuilder.ConfigureServices(
            (hostContext, services) => {
                var configuration = hostContext.Configuration;

                services.AddApplication();
                services.AddInfrastructure(
                    hostContext.HostingEnvironment,
                    configuration,
                    busCfg => {
                        busCfg.AddConsumer<FolderRequestsConsumer>();
                        busCfg.AddConsumer<UploadRequestsConsumer>();
                    }
                );
            }
        );
    }
}
