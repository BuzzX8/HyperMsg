using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class ServiceHost : IHost, IServiceProvider
    {
        private readonly ServiceProvider serviceProvider;

        public ServiceHost(IServiceCollection services) => serviceProvider = services.BuildServiceProvider();

        public IServiceProvider Services => serviceProvider;

        public object GetService(Type serviceType) => Services.GetService(serviceType);

        public void Dispose() => serviceProvider.Dispose();

        public void Start() => StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            var hostedServices = serviceProvider.GetServices<IHostedService>();

            foreach(var service in hostedServices)
            {
                await service.StartAsync(cancellationToken);
            }
        }

        public void Stop() => StopAsync().GetAwaiter().GetResult();

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            var hostedServices = serviceProvider.GetServices<IHostedService>();

            foreach (var service in hostedServices)
            {
                await service.StopAsync(cancellationToken);
            }
        }

        public static ServiceHost Create(Action<IServiceCollection> serviceConfigurator)
        {
            var services = new ServiceCollection();            
            serviceConfigurator.Invoke(services);

            return new ServiceHost(services);
        }

        public static ServiceHost CreateDefault(Action<IServiceCollection> serviceConfigurator = null)
        {
            return Create(services =>
            {
                services.AddMessagingServices();
                serviceConfigurator?.Invoke(services);
            });
        }
    }
}
