using Microsoft.Extensions.DependencyInjection;
using System.Buffers;

namespace HyperMsg.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds core services required for messaging and buffering infrastructure (MessageSender, MessageObservable,
        /// receiving and transmitting buffer).
        /// </summary>
        /// <param name="services"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static void AddCoreServices(this IServiceCollection services, int receivingBufferSize, int transmittingBufferSize)
        {
            services.AddSharedMemoryPool();
            services.AddBufferContext(receivingBufferSize, transmittingBufferSize);
            services.AddMessageBroker();            
        }

        /// <summary>
        /// Adds shared MemoryPool<byte> as service.
        /// </summary>
        /// <param name="services"></param>
        public static void AddSharedMemoryPool(this IServiceCollection services) => services.AddSingleton(MemoryPool<byte>.Shared);

        /// <summary>
        /// Adds implementations for IBufferContext. Depends on MemoryPool<byte>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static void AddBufferContext(this IServiceCollection services, int receivingBufferSize, int transmittingBufferSize)
        {
            services.AddSingleton(provider =>
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
        /// <param name="services"></param>
        public static void AddBufferFactory(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var memoryPool = provider.GetRequiredService<MemoryPool<byte>>();
                return new BufferFactory(memoryPool) as IBufferFactory;
            });
        }

        /// <summary>
        /// Adds implementation for services IMessagingContext, IMessageSender and IMessageObservable.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        public static void AddMessageBroker(this IServiceCollection services)
        {
            var broker = new MessageBroker();
            services.AddSingleton<IMessageSender>(broker);
            services.AddSingleton<IMessageObservable>(broker);
            services.AddSingleton<IMessagingContext>(broker);
        }
    }
}
