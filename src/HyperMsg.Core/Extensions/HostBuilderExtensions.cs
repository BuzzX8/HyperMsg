using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureObservers(this IHostBuilder hostBuilder, Action<IServiceProvider, IMessageObservable> configurationDelegate)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                if (!services.Any(s => s.ServiceType == typeof(ConfiguratorCollection)))
                {
                    services.AddSingleton(new ConfiguratorCollection());
                }

                var configurators = services.Single(s => s.ServiceType == typeof(ConfiguratorCollection)).ImplementationInstance as ConfiguratorCollection;
                configurators.Add(configurationDelegate);
                services.AddHostedService<HyperMsgBootstrapper>();                
            });
        }

        public static IHostBuilder ConfigureObservers<T>(this IHostBuilder hostBuilder, Action<T, IMessageObservable> configurationDelegate)
        {
            return hostBuilder.ConfigureObservers((provider, observable) =>
            {
                var component = provider.GetRequiredService<T>();
                configurationDelegate.Invoke(component, observable);
            });
        }
    }    

    internal class HyperMsgBootstrapper : IHostedService
    {
        private readonly IServiceProvider provider;
        private readonly ConfiguratorCollection configurators;

        public HyperMsgBootstrapper(IServiceProvider provider, ConfiguratorCollection configurators)
        {
            this.provider = provider;
            this.configurators = configurators;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var observable = provider.GetRequiredService<IMessageObservable>();
            foreach(var configurator in configurators)
            {
                configurator.Invoke(provider, observable);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    internal class ConfiguratorCollection : List<Action<IServiceProvider, IMessageObservable>> { }
}
