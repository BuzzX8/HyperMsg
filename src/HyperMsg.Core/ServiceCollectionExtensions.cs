using Microsoft.Extensions.DependencyInjection;
using System;
using System.Buffers;
using System.Linq;

namespace HyperMsg
{
    public static class ServiceCollectionExtensions
    {
        const int DefaultBufferSize = -1;

        /// <summary>
        /// Adds core services required for messaging and buffering infrastructure (MessageSender, MessageObservable,
        /// receiving and transmitting buffer).
        /// </summary>
        /// <param name="services"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static IServiceCollection AddMessagingServices(this IServiceCollection services, int receivingBufferSize = DefaultBufferSize, int transmittingBufferSize = DefaultBufferSize)
        {
            return services.AddBufferContext(receivingBufferSize, transmittingBufferSize)
                .AddBufferService()
                .AddSharedMemoryPool()
                .AddMessageBroker();
        }

        /// <summary>
        /// Adds shared MemoryPool<byte> as service.
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddSharedMemoryPool(this IServiceCollection services) => services.AddSingleton(MemoryPool<byte>.Shared);

        /// <summary>
        /// Adds implementations for IBufferContext. Depends on MemoryPool<byte>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static IServiceCollection AddBufferContext(this IServiceCollection services, int receivingBufferSize = DefaultBufferSize, int transmittingBufferSize = DefaultBufferSize)
        {
            return services.AddSingleton(provider =>
            {
                var memoryPool = provider.GetRequiredService<MemoryPool<byte>>();
                var receivingBuffer = new Buffer(memoryPool.Rent(receivingBufferSize));
                var transmittingBuffer = new Buffer(memoryPool.Rent(transmittingBufferSize));

                return new BufferContext(receivingBuffer, transmittingBuffer) as IBufferContext;
            });
        }

        public static IServiceCollection AddBufferService(this IServiceCollection services) => services.AddHostedService<BufferService>();

        /// <summary>
        /// Adds implementation for IBufferFactory. Depends on MemoryPool<byte>
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddBufferFactory(this IServiceCollection services) => services.AddSingleton(provider =>
        {
            var memoryPool = provider.GetRequiredService<MemoryPool<byte>>();
            return new BufferFactory(memoryPool) as IBufferFactory;
        });

        /// <summary>
        /// Adds implementation for services IMessagingContext, IMessageSender and IMessageObservable.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        public static IServiceCollection AddMessageBroker(this IServiceCollection services)
        {
            var broker = new MessageBroker();
            return services.AddSingleton<IMessageSender>(broker)
                .AddSingleton<IMessageHandlersRegistry>(broker)
                .AddSingleton<IMessagingContext>(broker);
        }

        public static IServiceCollection AddConfigurator(this IServiceCollection services, Action<IServiceProvider> configurator)
        {
            services.AddHostedService<ConfigurationService>(provider =>
            {
                var configurators = provider.GetRequiredService<ConfiguratorCollection>();
                return new (configurators, provider);
            });


            if (services.SingleOrDefault(s => s.ServiceType == typeof(ConfiguratorCollection))?.ImplementationInstance is not ConfiguratorCollection configurators)
            {
                configurators = new ConfiguratorCollection();
                services.AddSingleton(configurators);
            }

            configurators.Add(configurator);

            return services;
        }
    }
}