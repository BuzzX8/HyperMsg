using System;
using System.Buffers;

namespace HyperMsg
{
    public static class ServiceRegistryExtensions
    {
        public static void AddService<T>(this IServiceRegistry serviceRegistry, T serviceInstance) => serviceRegistry.Add(typeof(T), serviceInstance);

        public static void AddService<T>(this IServiceRegistry serviceRegistry, Func<IServiceProvider, T> serviceFactory)
        {
            serviceRegistry.Add(typeof(T), provider => serviceFactory(provider));
        }

        public static void AddComponentContainer(this IServiceRegistry serviceRegistry)
        {
            serviceRegistry.AddService(provider =>
            {
                var context = provider.GetRequiredService<IMessagingContext>();
                return new ComponentContainer(context);
            });
        }

        public static void AddMessagingComponent<T>(this IServiceRegistry serviceRegistry, T component) where T : IMessagingComponent
        {
            serviceRegistry.AddService(provider =>
            {
                var container = provider.GetRequiredService<ComponentContainer>();
                container.Add(component);
                return component;
            });
        }

        public static void AddSerializer<T>(this IServiceRegistry serviceRegistry, Action<IBufferWriter<byte>, T> serializer)
        {
            serviceRegistry.AddService(provider =>
            {
                var context = provider.GetRequiredService<IMessagingContext>();
                var bufferContext = provider.GetRequiredService<IBufferContext>();
                return new MessageSerializer<T>(context, bufferContext.TransmittingBuffer, serializer);
            });
        }
    }
}
