namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void AddService<T>(this IConfigurable configurable, T serviceInstance) => configurable.AddService(typeof(T), serviceInstance);
    }
}
