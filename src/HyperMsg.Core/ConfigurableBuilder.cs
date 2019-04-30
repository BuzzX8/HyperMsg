using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public abstract class ConfigurableBuilde<T> : IConfigurable
    {        
        private readonly List<Action<Configuration>> configurators;
        private readonly ServiceProviderFactory serviceProviderFactory;

        public ConfigurableBuilde(ServiceProviderFactory serviceProviderFactory)
        {
            this.serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
            configurators = new List<Action<Configuration>>();
        }

        public void Configure(Action<Configuration> configurator) => configurators.Add(configurator);

        public T Build()
        {
            var configuration = new Configuration();

            foreach (var configurator in configurators)
            {
                configurator.Invoke(configuration);
            }            

            var serviceProvider = serviceProviderFactory.Invoke(configuration.Services);

            return Build(serviceProvider, configuration);
        }

        protected abstract T Build(IServiceProvider serviceProvider, Configuration configuration);
    }
}
