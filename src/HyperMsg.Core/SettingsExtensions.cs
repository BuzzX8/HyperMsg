using System;
using System.Collections.Generic;
using System.Text;

namespace HyperMsg
{
    public static class SettingsExtensions
    {
        public static T Get<T>(this ISettings settings) => throw new NotImplementedException();

        public static void Set<T>(this ISettings settings, T value) => throw new NotImplementedException();

        public static bool TryGet<T>(this ISettings settings, out T value) => throw new NotImplementedException();
    }
}
