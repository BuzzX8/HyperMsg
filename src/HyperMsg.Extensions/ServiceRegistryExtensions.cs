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
