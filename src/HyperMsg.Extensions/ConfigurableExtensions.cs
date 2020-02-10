using System;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        class InstanceWrapper
        {
            readonly object serviceInstance;

            internal InstanceWrapper(object serviceInstance) => this.serviceInstance = serviceInstance;

            internal object GetInstance(IServiceProvider serviceProvider, IConfigurationSettings settings) => serviceInstance;
        }

        public static void RegisterService<T>(this IConfigurable configurable, T serviceInstance)
        {
            configurable.RegisterService(typeof(T), new InstanceWrapper(serviceInstance).GetInstance);
        }
    }
}
