using System;
using System.Collections.Generic;

namespace HyperMsg
{
    internal class ConfigurationContext : IConfigurationContext
    {
        private readonly Dictionary<string, object> settings;
        private readonly Dictionary<Type, object> services;
        private readonly Action<Type> resolveService;

        internal ConfigurationContext(Dictionary<string, object> settings, Dictionary<Type, object> services, Action<Type> resolveService)
        {
            this.settings = settings;
            this.services = services;
            this.resolveService = resolveService;
        }

        public object GetService(Type serviceType)
        {
            if (!services.ContainsKey(serviceType))
            {
                resolveService(serviceType);
            }

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
