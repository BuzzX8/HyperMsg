using System.Collections.Generic;

namespace HyperMsg
{
    internal class Settings : ISettings
    {
        private readonly Dictionary<string, object> values = new();

        public T Get<T>(string key)
        {
            key = $"{typeof(T).FullName}:{key}";

            if (!values.ContainsKey(key))
            {
                return default;
            }

            return (T)values[key];
        }

        public void Set<T>(string key, T value)
        {
            key = $"{typeof(T).FullName}:{key}";
            values[key] = value;
        }
    }
}
