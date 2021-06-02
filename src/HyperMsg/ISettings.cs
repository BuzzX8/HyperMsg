namespace HyperMsg
{
    public interface ISettings
    {
        T Get<T>(string key);

        void Set<T>(string key, T value);
    }
}
