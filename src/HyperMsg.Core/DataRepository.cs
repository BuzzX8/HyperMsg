using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HyperMsg
{
    internal class DataRepository : IDataRepository
    {
        private readonly ConcurrentDictionary<object, object> values = new();

        public T Get<T>(object key)
        {
            key = $"{typeof(T).FullName}:{key}";

            if (!values.ContainsKey(key))
            {
                return default;
            }

            return (T)values[key];
        }

        public void AddOrUpdate<T>(object key, T value)
        {
            key = $"{typeof(T).FullName}:{key}";
            values[key] = value;
        }

        public void Remove<T>(object key)
        {

        }

        public bool Contains<T>(object key)
        {
            key = $"{typeof(T).FullName}:{key}";
            return values.ContainsKey(key);
        }

        public void Clear() => values.Clear();
    }
}
