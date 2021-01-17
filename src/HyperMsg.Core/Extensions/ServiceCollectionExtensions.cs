using Microsoft.Extensions.DependencyInjection;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace HyperMsg.Extensions
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
                .AddSingleton<IMessageObservable>(broker)
                .AddSingleton<IMessagingContext>(broker);
        }

        public static IServiceCollection AddSerializer<TMessage>(this IServiceCollection services, Action<IBufferWriter<byte>, TMessage> serializer)
        {
            var initializers = services.AddSerializationService();
            initializers.Add(service => service.AddSerializer(serializer));
            return services;
        }

        public static IServiceCollection AddDeserializer<TMessage>(this IServiceCollection services, Func<ReadOnlySequence<byte>, (int BytesRead, TMessage Message)> deserializer)
        {
            var initializers = services.AddSerializationService();
            initializers.Add(service => service.AddDeserializer(deserializer));
            return services;
        }

        private static SerializationServiceInitializers AddSerializationService(this IServiceCollection services)
        {
            return services.AddHostedService(provider =>
            {
                var initializers = provider.GetRequiredService<SerializationServiceInitializers>();
                var messagingContext = provider.GetRequiredService<IMessagingContext>();
                var bufferContext = provider.GetRequiredService<IBufferContext>();
                return new SerializationService(initializers, messagingContext, bufferContext.TransmittingBuffer);
            }).AddSerializationServiceInitializers();
        }

        private static SerializationServiceInitializers AddSerializationServiceInitializers(this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType == typeof(SerializationServiceInitializers)))
            {
                return (SerializationServiceInitializers)services.Single(s => s.ServiceType == typeof(SerializationServiceInitializers)).ImplementationInstance;
            }

            var initializers = new SerializationServiceInitializers();
            services.AddSingleton(initializers);
            return initializers;
        }
    }
}
