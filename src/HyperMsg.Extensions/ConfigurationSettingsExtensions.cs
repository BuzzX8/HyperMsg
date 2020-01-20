using System;

namespace HyperMsg
{
    public static class ConfigurationSettingsExtensions
    {
        public static T Get<T>(this IConfigurationSettings settings, string key)
        {
            var value = settings[key];

            if (value.GetType() != typeof(T))
            {
                throw new InvalidOperationException();
            }

            return (T)value;
        }        
    }
}
