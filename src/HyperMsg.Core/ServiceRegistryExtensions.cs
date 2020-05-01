using System.Buffers;

namespace HyperMsg
{
    public static class ServiceRegistryExtensions
    {
        /// <summary>
        /// Adds core services required for messaging and buffering infrastructure (MessageSender, MessageObservable,
        /// receiving and transmitting buffer).
        /// </summary>
        /// <param name="serviceRegistry"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static void AddCoreServices(this IServiceRegistry serviceRegistry, int receivingBufferSize, int transmittingBufferSize)
        {
            serviceRegistry.AddSharedMemoryPool();
            serviceRegistry.AddBufferContext(receivingBufferSize, transmittingBufferSize);
            serviceRegistry.AddMessageBroker();
        }

        /// <summary>
        /// Adds shared MemoryPool<byte> as service.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        public static void AddSharedMemoryPool(this IServiceRegistry serviceRegistry) => serviceRegistry.AddService(MemoryPool<byte>.Shared);

        /// <summary>
        /// Adds implementations for IBufferContext. Depends on MemoryPool<byte>.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static void AddBufferContext(this IServiceRegistry serviceRegistry, int receivingBufferSize, int transmittingBufferSize)
        {
            serviceRegistry.AddService(provider =>
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
        /// <param name="serviceRegistry"></param>
        public static void AddBufferFactory(this IServiceRegistry serviceRegistry)
        {
            serviceRegistry.AddService(provider =>
            {
                var memoryPool = provider.GetRequiredService<MemoryPool<byte>>();
                return new BufferFactory(memoryPool) as IBufferFactory;
            });
        }

        /// <summary>
        /// Adds implementation for services IMessagingContext, IMessageSender and IMessageObservable.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        public static void AddMessageBroker(this IServiceRegistry serviceRegistry)
        {
            var broker = new MessageBroker();
            serviceRegistry.AddService<IMessageSender>(broker);
            serviceRegistry.AddService<IMessageObservable>(broker);
            serviceRegistry.AddService<IMessagingContext>(broker);
        }
    }
}
