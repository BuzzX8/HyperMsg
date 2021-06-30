using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace HyperMsg
{
    internal class DataRepository : IDataRepository, IDisposable
    {
        private readonly ConcurrentDictionary<(Type type, object key), object> values = new();

        public T Get<T>(object key)
        {
            if (!values.TryGetValue((typeof(T), key), out var value))
            {
                return default;
            }

            return (T)value;
        }

        public IEnumerable<(object key, T value)> GetAll<T>() => values.Where(kvp => kvp.Key.type == typeof(T)).Select(kvp => (kvp.Key.key, (T)kvp.Value));

        public void AddOrReplace<T>(object key, T value) => values.AddOrUpdate((typeof(T), key), value, (key, oldValue) => value);

        public void Remove<T>(object key) => values.Remove((typeof(T), key), out var _);

        public void RemoveAll<T>()
        {
            var keys = values.Where(kvp => kvp.Key.type == typeof(T)).Select(kvp => kvp.Key).ToArray();

            foreach(var key in keys)
            {
                values.TryRemove(key, out _);
            }
        }

        public bool Contains<T>(object key) => values.ContainsKey((typeof(T), key));

        public void Clear() => values.Clear();

        public void Dispose() => Clear();
    }
}
