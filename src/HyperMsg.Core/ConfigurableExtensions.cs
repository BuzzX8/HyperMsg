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
            configurable.UseBufferContext(receivingBufferSize, transmittingBufferSize);
            configurable.UseMessageBroker();
        }

        /// <summary>
        /// Registers shared MemoryPool<byte>.
        /// </summary>
        /// <param name="configurable"></param>
        public static void UseSharedMemoryPool(this IConfigurable configurable) => configurable.RegisterService(MemoryPool<byte>.Shared);

        /// <summary>
        /// Registers implementations for IBufferContext. Depends on MemoryPool<byte>.
        /// </summary>
        /// <param name="configurable"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static void UseBufferContext(this IConfigurable configurable, int receivingBufferSize, int transmittingBufferSize)
        {
            const string ReceivingBufferSetting = "ReceivingBufferSize";
            const string TransmittingBufferSetting = "TransmittingBufferSize";

            configurable.AddSetting(ReceivingBufferSetting, receivingBufferSize);
            configurable.AddSetting(TransmittingBufferSetting, transmittingBufferSize);
            configurable.RegisterService(typeof(IBufferContext), (p, s) =>
            {
                var inputBufferSize = s.Get<int>(ReceivingBufferSetting);
                var outputBufferSize = s.Get<int>(TransmittingBufferSetting);
                var memoryPool = p.GetRequiredService<MemoryPool<byte>>();

                var receivingBuffer = new Buffer(memoryPool.Rent(inputBufferSize));
                var transmittingBuffer = new Buffer(memoryPool.Rent(outputBufferSize));

                return new BufferContext(receivingBuffer, transmittingBuffer);
            });
        }

        /// <summary>
        /// Registers implementation for IBufferFactory. Depends on MemoryPool<byte>
        /// </summary>
        /// <param name="configurable"></param>
        public static void UseBufferFactory(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IBufferFactory), (p, s) =>
            {
                var memoryPool = p.GetRequiredService<MemoryPool<byte>>();
                return new BufferFactory(memoryPool);
            });
        }

        /// <summary>
        /// Register implementation for services IMessageSender and IMessageHandlerRegistry.
        /// </summary>
        /// <param name="configurable"></param>
        public static void UseMessageBroker(this IConfigurable configurable)
        {
            configurable.RegisterService(new[] { typeof(IMessageSender), typeof(IMessageHandlerRegistry) }, (p, s) => new MessageBroker());            
        }
    }
}
