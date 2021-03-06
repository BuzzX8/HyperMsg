﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                .AddBufferTransferingService()
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

        public static IServiceCollection AddBufferTransferingService(this IServiceCollection services)
        {
            return services.AddHostedService(provider =>
            {
                var messagingContext = provider.GetRequiredService<IMessagingContext>();
                var bufferContext = provider.GetRequiredService<IBufferContext>();
                return new BufferTransferingService(messagingContext, bufferContext.TransmittingBuffer);
            }).AddSingleton(provider => (BufferTransferingService)provider.GetServices<IHostedService>().Single(s => s is BufferTransferingService));
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

        public static IServiceCollection AddTransmittingBufferSerializer<TMessage>(this IServiceCollection services, Action<IBufferWriter<byte>, TMessage> serializer)
        {
            services.AddBufferTransferingService();
            return services.AddConfigurator(provider =>
            {
                var service = provider.GetRequiredService<BufferTransferingService>();
                service.AddTransmittingBufferSerializer(serializer);
            });
        }

        public static IServiceCollection AddReceivingBufferDeserializer<TMessage>(this IServiceCollection services, Func<ReadOnlySequence<byte>, (int BytesRead, TMessage Message)> deserializer)
        {
            services.AddBufferTransferingService();
            return services.AddConfigurator(provider =>
            {
                var service = provider.GetRequiredService<BufferTransferingService>();
                service.AddReceivingBufferDeserializer(deserializer);
            });
        }

        public static IServiceCollection AddReceivingBufferReader(this IServiceCollection services, Func<ReadOnlySequence<byte>, int> reader)
        {
            return services.AddBufferTransferingService()
                .AddConfigurator(provider =>
                {
                    var service = provider.GetRequiredService<BufferTransferingService>();
                    service.AddReceivingBufferReader(reader);
                });
        }

        public static IServiceCollection AddReceivingBufferReader(this IServiceCollection services, Func<ReadOnlySequence<byte>, CancellationToken, Task<int>> reader)
        {
            return services.AddBufferTransferingService()
                .AddConfigurator(provider =>
                {
                    var service = provider.GetRequiredService<BufferTransferingService>();
                    service.AddReceivingBufferReader(reader);
                });
        }

        public static IServiceCollection AddTimerService(this IServiceCollection services) => services.AddHostedService<TimerService>();

        public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, Action<T> messageHandler)
        {
            return services.AddSingleton<DisposalService>()
                .AddConfigurator(provider =>
                {
                    var registry = provider.GetRequiredService<IMessageHandlersRegistry>();
                    var disService = provider.GetRequiredService<DisposalService>();

                    disService.AddDisposable(registry.RegisterHandler(messageHandler));
                });            
        }

        public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, Func<T, bool> predicate, Action messageHandler)
            => services.AddMessageHandler<T>(predicate, _ => messageHandler.Invoke());

        public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, Func<T, bool> predicate, Action<T> messageHandler)
        {
            return services.AddMessageHandler<T>(m =>
            {
                if (predicate.Invoke(m))
                {
                    messageHandler.Invoke(m);
                }
            });
        }

        public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, T message, Action messageHandler)
            => services.AddMessageHandler<T>(m => message.Equals(m), messageHandler);

        public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, T message, Action<T> messageHandler)
            => services.AddMessageHandler(m => message.Equals(m), messageHandler);

        public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, AsyncAction<T> messageHandler)
        {
            return services.AddSingleton<DisposalService>()
                .AddConfigurator(provider =>
                {
                    var registry = provider.GetRequiredService<IMessageHandlersRegistry>();
                    var disService = provider.GetRequiredService<DisposalService>();

                    disService.AddDisposable(registry.RegisterHandler(messageHandler));
                });
        }

        public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, Func<T, bool> predicate, AsyncAction messageHandler)
            => services.AddMessageHandler<T>(predicate, (_, t) => messageHandler.Invoke(t));

        public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, Func<T, bool> predicate, AsyncAction<T> messageHandler)
        {
            return services.AddMessageHandler<T>((m, t) =>
            {
                if (predicate.Invoke(m))
                {
                    return messageHandler.Invoke(m, t);
                }

                return Task.CompletedTask;
            });
        }

        public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, T message, AsyncAction messageHandler)
            => services.AddMessageHandler<T>(m => message.Equals(m), messageHandler);

        public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, T message, AsyncAction<T> messageHandler)
            => services.AddMessageHandler(m => message.Equals(m), messageHandler);
    }
}
