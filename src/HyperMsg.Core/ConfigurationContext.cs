using System;
using System.Collections.Generic;

namespace HyperMsg
{
    internal class ConfigurationContext : IConfigurationContext
    {
        private readonly Dictionary<string, object> settings;        
        private readonly Action<Type> requireService;

        internal readonly Dictionary<Type, object> Services;

        internal ConfigurationContext(Action<Type> requireService)
        {
            this.requireService = requireService;
            Services = new Dictionary<Type, object>();
        }

        public object GetService(Type serviceType)
        {
            if (!Services.ContainsKey(serviceType))
            {
                requireService(serviceType);
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
