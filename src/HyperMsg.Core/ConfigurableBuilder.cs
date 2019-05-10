using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public class ConfigurableBuilder<T> : IConfigurable
    {        
        private readonly List<Action<Configuration>> configurators;
        private readonly List<(Action<Configuration, object>, object)> parametrizedConfigurators;
        private readonly ServiceProviderFactory serviceProviderFactory;

        public ConfigurableBuilder(ServiceProviderFactory serviceProviderFactory)
        {
            this.serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
            configurators = new List<Action<Configuration>>();
            parametrizedConfigurators = new List<(Action<Configuration, object>, object)>();
        }

        public void Configure(Action<Configuration> configurator) => configurators.Add(configurator);

        public void Configure(Action<Configuration, object> configurator, object settings)
        {
            parametrizedConfigurators.Add((configurator, settings));
        }

        public T Build()
        {
            var configuration = new Configuration();

            foreach (var configurator in configurators)
            {
                configurator.Invoke(configuration);
            }

            foreach (var configurator in parametrizedConfigurators)
            {
                configurator.Item1(configuration, configurator.Item2);
            }

            var serviceProvider = serviceProviderFactory.Invoke(configuration.Services);

            return (T)serviceProvider.GetService(typeof(T));
        }
    }
}
