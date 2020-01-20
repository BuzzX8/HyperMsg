using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public static class ConfigurationSettingsExtensions
    {
        public static T Get<T>(this IConfigurationSettings settings, string key)
        {
            var value = settings[key];

            if (!settings.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            if (value.GetType() != typeof(T))
            {
                throw new InvalidOperationException();
            }

            return (T)value;
        }        
    }
}
