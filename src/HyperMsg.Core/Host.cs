using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class Host : IHost
    {
        private readonly ServiceProvider serviceProvider;

        public Host(IServiceCollection services) => serviceProvider = services.BuildServiceProvider();

        public IServiceProvider Services => serviceProvider;

        public void Dispose() => serviceProvider.Dispose();

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            var hostedServices = serviceProvider.GetServices<IHostedService>();

            foreach(var service in hostedServices)
            {
                await service.StartAsync(cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            var hostedServices = serviceProvider.GetServices<IHostedService>();

            foreach (var service in hostedServices)
            {
                await service.StopAsync(cancellationToken);
            }
        }
    }
}
