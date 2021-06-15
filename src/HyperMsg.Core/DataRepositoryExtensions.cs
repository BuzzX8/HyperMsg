namespace HyperMsg
{
    public static class DataRepositoryExtensions
    {
        public static T Get<T>(this IDataRepository settings) => settings.Get<T>(GetTypeKey<T>());

        public static void AddOrUpdate<T>(this IDataRepository settings, T value) => settings.AddOrUpdate(GetTypeKey<T>(), value);

        public static bool TryGet<T>(this IDataRepository settings, out T value) => settings.TryGet(GetTypeKey<T>(), out value);

        public static bool TryGet<T>(this IDataRepository settings, string key, out T value)
        {
            value = settings.Get<T>(key);

            return !Equals(value, default(T));
        }

        private static string GetTypeKey<T>() => typeof(T).GUID.ToString();
    }
}
