using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal class ConfigurationService : IHostedService
    {
        private readonly ConfiguratorCollection configurators;
        private readonly IServiceProvider serviceProvider;

        public ConfigurationService(ConfiguratorCollection configurators, IServiceProvider serviceProvider)
        {
            this.configurators = configurators;
            this.serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            configurators.ForEach(c => c.Invoke(serviceProvider));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    internal class ConfiguratorCollection : List<Action<IServiceProvider>> { }
}
