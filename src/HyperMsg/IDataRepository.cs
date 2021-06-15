namespace HyperMsg
{
    public interface IDataRepository
    {
        T Get<T>(string key);

        void Set<T>(string key, T value);
    }
}
