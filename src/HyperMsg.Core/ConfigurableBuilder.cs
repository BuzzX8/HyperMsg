using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public class ConfigurableBuilder<T> : IConfigurable
    {        
        private readonly List<Action<Configuration>> configurators;
        private readonly List<(Action<Configuration, object>, object)> parametrizedConfigurators;
        private readonly ServiceProviderFactory serviceProviderFactory;

        private readonly Dictionary<string, object> settings;

        public ConfigurableBuilder(ServiceProviderFactory serviceProviderFactory)
        {
            this.serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
            configurators = new List<Action<Configuration>>();
            parametrizedConfigurators = new List<(Action<Configuration, object>, object)>();
            settings = new Dictionary<string, object>();
        }

        public void AddSetting(string settingName, object setting) => settings.Add(settingName, setting);

        public void Configure(Action<Configuration> configurator) => configurators.Add(configurator);

        public T Build()
        {
            var configuration = new Configuration(new List<ServiceDescriptor>(), settings);            

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
