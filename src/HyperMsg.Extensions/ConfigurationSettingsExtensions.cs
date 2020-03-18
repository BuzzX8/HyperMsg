using HyperMsg.Properties;
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
            var valueType = value.GetType();
            var targetType = typeof(T);

            if (!targetType.IsAssignableFrom(valueType))
            {
                var message = string.Format(Resources.HyperMsg_InvalidConfigurationSettingType, key, valueType, targetType);
                throw new InvalidOperationException(message);
            }

            return (T)value;
        }        
    }
}
