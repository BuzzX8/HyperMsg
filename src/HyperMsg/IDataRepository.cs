using System.Collections.Generic;

namespace HyperMsg
{
    public interface IDataRepository
    {
        T Get<T>(object key);

        IEnumerable<(object key, T value)> GetAll<T>();

        void AddOrReplace<T>(object key, T value);

        void Remove<T>(object key);

        void RemoveAll<T>();

        bool Contains<T>(object key);

        void Clear();
    }
}
