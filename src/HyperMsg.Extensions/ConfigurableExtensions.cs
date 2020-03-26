using System;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void AddService<T>(this IConfigurable configurable, T serviceInstance) => configurable.AddService(typeof(T), serviceInstance);

        public static void AddService<T>(this IConfigurable configurable, Func<IServiceProvider, T> serviceFactory)
        {
            configurable.AddService(typeof(T), provider => serviceFactory(provider));
        }
    }
}
