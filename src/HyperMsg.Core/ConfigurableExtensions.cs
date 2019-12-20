using System.Buffers;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        /// <summary>
        /// Registers core services required for messaging and buffering infrastructure (MessageSender, MessageHandlerRegistry,
        /// receiving and transmitting buffer).
        /// </summary>
        /// <param name="configurable"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static void UseCoreServices(this IConfigurable configurable, int receivingBufferSize, int transmittingBufferSize)
        {
            configurable.UseSharedMemoryPool();
            configurable.UseBuffers(receivingBufferSize, transmittingBufferSize);
            configurable.UseMessageBroker();
        }

        /// <summary>
        /// Registers shared MemoryPool<byte>.
        /// </summary>
        /// <param name="configurable"></param>
        public static void UseSharedMemoryPool(this IConfigurable configurable) => configurable.RegisterService(typeof(MemoryPool<byte>), (p, s) => MemoryPool<byte>.Shared);

        /// <summary>
        /// Registers implementations for ITransmittingBuffer and IReceivingBuffer. Depends on MemoryPool<byte>.
        /// </summary>
        /// <param name="configurable"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
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

        /// <summary>
        /// Register implementation for services IMessageSender and IMessageHandlerRegistry.
        /// </summary>
        /// <param name="configurable"></param>
        public static void UseMessageBroker(this IConfigurable configurable)
        {
            configurable.RegisterService(new[] { typeof(IMessageSender), typeof(IMessageHandlerRegistry) }, (p, s) =>
            {
                return new MessageBroker();
            });
        }
    }
}
