using System.Buffers;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        /// <summary>
        /// Adds core services required for messaging and buffering infrastructure (MessageSender, MessageObservable,
        /// receiving and transmitting buffer).
        /// </summary>
        /// <param name="configurable"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static void AddCoreServices(this IConfigurable configurable, int receivingBufferSize, int transmittingBufferSize)
        {
            configurable.AddSharedMemoryPool();
            configurable.AddBufferContext(receivingBufferSize, transmittingBufferSize);
            configurable.AddMessageBroker();
        }

        /// <summary>
        /// Adds shared MemoryPool<byte> as service.
        /// </summary>
        /// <param name="configurable"></param>
        public static void AddSharedMemoryPool(this IConfigurable configurable) => configurable.AddService(MemoryPool<byte>.Shared);

        /// <summary>
        /// Adds implementations for IBufferContext. Depends on MemoryPool<byte>.
        /// </summary>
        /// <param name="configurable"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static void AddBufferContext(this IConfigurable configurable, int receivingBufferSize, int transmittingBufferSize)
        {
            configurable.AddService(provider =>
            {
                var memoryPool = provider.GetRequiredService<MemoryPool<byte>>();

                var receivingBuffer = new Buffer(memoryPool.Rent(receivingBufferSize));
                var transmittingBuffer = new Buffer(memoryPool.Rent(transmittingBufferSize));

                return new BufferContext(receivingBuffer, transmittingBuffer) as IBufferContext;
            });
        }

        /// <summary>
        /// Adds implementation for IBufferFactory. Depends on MemoryPool<byte>
        /// </summary>
        /// <param name="configurable"></param>
        public static void AddBufferFactory(this IConfigurable configurable)
        {
            configurable.AddService(provider =>
            {
                var memoryPool = provider.GetRequiredService<MemoryPool<byte>>();
                return new BufferFactory(memoryPool) as IBufferFactory;
            });
        }

        /// <summary>
        /// Adds implementation for services IMessageSender and IMessageObservable.
        /// </summary>
        /// <param name="configurable"></param>
        public static void AddMessageBroker(this IConfigurable configurable)
        {
            var broker = new MessageBroker();
            configurable.AddService<IMessageSender>(broker);
            configurable.AddService<IMessageObservable>(broker);
        }
    }
}
