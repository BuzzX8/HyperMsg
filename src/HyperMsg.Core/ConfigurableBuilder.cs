using System;
using System.Collections.Generic;
using Configurator = System.Action<HyperMsg.IConfigurationContext>;

namespace HyperMsg
{
    public class ConfigurableBuilder<T> : IConfigurable
    {        
        private readonly List<Configurator> configurators;
        private readonly Dictionary<string, object> settings;
        private readonly Lazy<T> service;        

        public ConfigurableBuilder()
        {
            configurators = new List<Configurator>();
            settings = new Dictionary<string, object>();
            service = new Lazy<T>(BuildService);
        }

        public void AddSetting(string settingName, object setting) => settings.Add(settingName, setting);

        public void Configure(Configurator configurator) => configurators.Add(configurator);

        public T Build() => service.Value;

        private T BuildService()
        {
            var resolver = new ConfigurationTaskRunner(configurators);
            resolver.RunConfiguration(settings);
            return resolver.GetService<T>();
        }
    }
}
