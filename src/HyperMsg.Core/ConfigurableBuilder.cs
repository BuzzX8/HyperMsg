using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public class ConfigurableBuilder<T> : IConfigurable
    {        
        private readonly List<Action<Configuration>> configurators;
        private readonly ServiceProviderFactory serviceProviderFactory;

        public ConfigurableBuilder(ServiceProviderFactory serviceProviderFactory)
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

            return (T)serviceProvider.GetService(typeof(T));
        }
    }
}
