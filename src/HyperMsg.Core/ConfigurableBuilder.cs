using System;
using System.Collections.Generic;
using Configurator = System.Action<HyperMsg.IConfigurationContext>;

namespace HyperMsg
{
    public class ConfigurableBuilder<T> : IConfigurable
    {        
        private readonly Queue<Configurator> configurators;
        private readonly Dictionary<string, object> settings;
                
        private Configurator currentConfigurator;
        private ConfigurationContext currentContext;

        public ConfigurableBuilder()
        {
            configurators = new Queue<Configurator>();
            settings = new Dictionary<string, object>();
        }

        public void AddSetting(string settingName, object setting) => settings.Add(settingName, setting);

        public void Configure(Configurator configurator) => configurators.Enqueue(configurator);

        public T Build()
        {
            currentContext = new ConfigurationContext(settings, ResolveService);
            InvokeNextConfigurator();

            return (T)currentContext.GetService(typeof(T));
        }

        private void ResolveService(Type serviceType)
        {
            InvokeNextConfigurator();

            if (!currentContext.Services.ContainsKey(serviceType))
            {
                ResolveService(serviceType);
            }
        }

        private void InvokeNextConfigurator()
        {
            while (configurators.Count > 0)
            {
                currentConfigurator = configurators.Dequeue();
                currentConfigurator.Invoke(currentContext);
            }
        }
    }
}
