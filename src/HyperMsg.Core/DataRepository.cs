using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HyperMsg
{
    internal class DataRepository : IDataRepository, IDisposable
    {
        private readonly ConcurrentDictionary<(Type, object), object> values = new();

        public T Get<T>(object key)
        {
            if (!values.TryGetValue((typeof(T), key), out var value))
            {
                return default;
            }

            return (T)value;
        }

        public void AddOrReplace<T>(object key, T value) => values.AddOrUpdate((typeof(T), key), value, (key, oldValue) => value);

        public void Remove<T>(object key) => values.Remove((typeof(T), key), out var _);

        public bool Contains<T>(object key) => values.ContainsKey((typeof(T), key));

        public void Clear() => values.Clear();

        public void Dispose() => Clear();
    }
}
