namespace HyperMsg
{
    public interface IDataRepository
    {
        T Get<T>(object key);

        void AddOrUpdate<T>(object key, T value);

        void Remove<T>(object key);

        bool Contains<T>(object key);

        void Clear();
    }
}
