using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public class ConfigurableBuilder<T> : IConfigurable
    {        
        private readonly List<Action<IConfigurationContext>> configurators;
        private readonly Dictionary<string, object> settings;

        public ConfigurableBuilder()
        {
            configurators = new List<Action<IConfigurationContext>>();
            settings = new Dictionary<string, object>();
        }

        public void AddSetting(string settingName, object setting) => settings.Add(settingName, setting);

        public void Configure(Action<IConfigurationContext> configurator) => configurators.Add(configurator);

        public T Build()
        {
            var context = new ConfigurationContext();

            foreach (var configurator in configurators)
            {
                configurator.Invoke(context);
            }

            return (T)context.GetService(typeof(T));
        }
    }
}
