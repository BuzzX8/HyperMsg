using System;

namespace HyperMsg
{
    public static class SettingsExtensions
    {
        public static T Get<T>(this ISettings settings) => settings.Get<T>(GetTypeKey<T>());

        public static void Set<T>(this ISettings settings, T value) => settings.Set(GetTypeKey<T>(), value);

        public static bool TryGet<T>(this ISettings settings, out T value) => settings.TryGet(GetTypeKey<T>(), out value);

        public static bool TryGet<T>(this ISettings settings, string key, out T value)
        {
            value = settings.Get<T>(key);

            return !Equals(value, default(T));
        }

        private static string GetTypeKey<T>() => typeof(T).GUID.ToString();
    }
}
