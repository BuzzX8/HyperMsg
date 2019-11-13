using System.Buffers;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void UseCoreServices<T>(this IConfigurable configurable, int receivingBufferSize, int transmittingBufferSize)
        {
            configurable.UseSharedMemoryPool();
            configurable.UseBuffers(receivingBufferSize, transmittingBufferSize);
        }

        public static void UseSharedMemoryPool(this IConfigurable configurable) => configurable.RegisterService(typeof(MemoryPool<byte>), (p, s) => MemoryPool<byte>.Shared);

        public static void UseBuffers(this IConfigurable configurable, int receivingBufferSize, int transmittingBufferSize)
        {
            const string ReceivingBufferSetting = "ReceivingBufferSize";
            const string SendingBufferSetting = "SendingBufferSize";

            configurable.AddSetting(ReceivingBufferSetting, receivingBufferSize);
            configurable.AddSetting(SendingBufferSetting, transmittingBufferSize);
            configurable.RegisterService(typeof(IReceivingBuffer), (p, s) =>
            {
                var bufferSize = (int)s[ReceivingBufferSetting];
                var memoryPool = (MemoryPool<byte>)p.GetService(typeof(MemoryPool<byte>));

                return new Buffer(memoryPool.Rent(bufferSize));
            });
            configurable.RegisterService(typeof(ITransmittingBuffer), (p, s) =>
            {
                var bufferSize = (int)s[SendingBufferSetting];
                var memoryPool = (MemoryPool<byte>)p.GetService(typeof(MemoryPool<byte>));

                return new Buffer(memoryPool.Rent(bufferSize));
            });
        }

        public static void UseMessageBroker(this IConfigurable configurable)
        {
            configurable.RegisterService(new[] { typeof(IMessageSender), typeof(IMessageHandlerRegistry) }, (p, s) =>
            {
                return new MessageBroker();
            });
        }
    }
}
