﻿using System;

namespace HyperMsg
{
    public static class DataRepositoryExtensions
    {
        public static T Get<T>(this IDataRepository repository) => repository.Get<T>(GetTypeKey<T>());

        public static void AddOrReplace<T>(this IDataRepository repository, T value) => repository.AddOrReplace(GetTypeKey<T>(), value);

        public static bool TryGet<T>(this IDataRepository repository, out T value) => repository.TryGet(GetTypeKey<T>(), out value);

        public static bool TryGet<T>(this IDataRepository repository, object key, out T value)
        {
            value = default;

            if (!repository.Contains<T>(key))
            {
                return false;
            }

            value = repository.Get<T>(key);
            return true;
        }

        private static Guid GetTypeKey<T>() => typeof(T).GUID;
    }
}
