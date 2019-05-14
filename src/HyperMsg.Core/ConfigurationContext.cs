using System;
using System.Collections.Generic;

namespace HyperMsg
{
    internal class ConfigurationContext : IConfigurationContext
    {
        private readonly Dictionary<string, object> settings;
        private readonly Action<Type> resolveService;

        internal readonly Dictionary<Type, object> Services;

        internal ConfigurationContext(Dictionary<string, object> settings, Action<Type> resolveService)
        {
            this.settings = settings;
            this.resolveService = resolveService;
            Services = new Dictionary<Type, object>();            
        }

        public object GetService(Type serviceType)
        {
            if (!Services.ContainsKey(serviceType))
            {
                resolveService(serviceType);
            }

            return Services[serviceType];
        }

        public object GetSetting(string settingName)
        {
            return settings[settingName];
        }

        public void RegisterService(Type serviceType, object serviceInstance)
        {
            Services.Add(serviceType, serviceInstance);
        }
    }
}
