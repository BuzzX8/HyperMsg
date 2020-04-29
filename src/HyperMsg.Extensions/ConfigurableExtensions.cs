using System;
using System.Buffers;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void AddService<T>(this IConfigurable configurable, T serviceInstance) => configurable.AddService(typeof(T), serviceInstance);

        public static void AddService<T>(this IConfigurable configurable, Func<IServiceProvider, T> serviceFactory)
        {
            configurable.AddService(typeof(T), provider => serviceFactory(provider));
        }

        public static void AddSerializer<T>(this IConfigurable configurable, Action<IBufferWriter<byte>, T> serializer)
        {
            configurable.AddService(provider =>
            {
                var context = provider.GetRequiredService<IMessagingContext>();
                var bufferContext = provider.GetRequiredService<IBufferContext>();
                return new MessageSerializer<T>(context, bufferContext.TransmittingBuffer, serializer);
            });
        }
    }
}
