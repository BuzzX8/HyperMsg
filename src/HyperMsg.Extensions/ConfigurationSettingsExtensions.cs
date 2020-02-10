using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public static class ConfigurationSettingsExtensions
    {
        public static T Get<T>(this IConfigurationSettings settings, string key)
        {
            if (!settings.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            var value = settings[key];

            if (value.GetType() != typeof(T))
            {
                throw new InvalidOperationException();
            }

            return (T)value;
        }        
    }
}
