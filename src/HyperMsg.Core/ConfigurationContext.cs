using System;
using System.Collections.Generic;

namespace HyperMsg
{
    internal class ConfigurationContext : IConfigurationContext
    {
        private readonly Dictionary<string, object> settings;
        private readonly Dictionary<Type, object> services;

        internal ConfigurationContext()
        {
            services = new Dictionary<Type, object>();
        }

        public object GetService(Type serviceType)
        {
            return services[serviceType];
        }

        public object GetSetting(string settingName)
        {
            return settings[settingName];
        }

        public void RegisterService(Type serviceType, object serviceInstance)
        {
            services.Add(serviceType, serviceInstance);
        }
    }
}
