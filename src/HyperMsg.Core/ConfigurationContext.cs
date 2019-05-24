using System;
using System.Collections.Generic;

namespace HyperMsg
{
    internal class ConfigurationContext : IConfigurationContext
    {
        private readonly Dictionary<string, object> settings;
        private readonly ConfigurationTaskRunner resolver;

        internal ConfigurationContext(Dictionary<string, object> settings, ConfigurationTaskRunner resolver)
        {
            this.settings = settings;
            this.resolver = resolver;
        }

        public object GetService(Type serviceType)
        {
            return resolver.ResolveDependencyAsync(serviceType).Result;
        }

        public object GetSetting(string settingName)
        {
            return settings[settingName];
        }

        public void RegisterService(Type serviceType, object serviceInstance)
        {
            resolver.RegisterDependency(serviceType, serviceInstance);
        }
    }
}
