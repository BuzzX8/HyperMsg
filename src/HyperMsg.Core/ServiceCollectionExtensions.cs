﻿using Microsoft.Extensions.DependencyInjection;
using System.Buffers;

namespace HyperMsg;

public static class ServiceCollectionExtensions
{
    const int DefaultBufferSize = -1;

    /// <summary>
    /// Adds core services required for messaging and buffering infrastructure.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="receivingBufferSize">Size of receiving buffer.</param>
    /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
    public static IServiceCollection AddMessagingServices(this IServiceCollection services, int receivingBufferSize = DefaultBufferSize, int transmittingBufferSize = DefaultBufferSize)
    {
        return services
            .AddBufferContext(receivingBufferSize, transmittingBufferSize)
            .AddSharedMemoryPool()
            .AddMessageBroker()
            .AddHandlersRegistration()
            .WireBaseInterfaces();
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
            var sender = provider.GetRequiredService<MessageBroker>();

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
    /// 
    /// </summary>
    /// <param name="serviceRegistry"></param>
    public static IServiceCollection AddMessageBroker(this IServiceCollection services) => services.AddSingleton<MessageBroker>();

    private static IServiceCollection AddHandlersRegistration(this IServiceCollection services)
    {
        return services.AddSingleton<HandlersRegistratorList>()
            .AddHostedService<HandlersRegistrationService>();
    }

    private static IServiceCollection WireBaseInterfaces(this IServiceCollection services)
    {
        return services
            .AddSingleton(provider => provider.GetRequiredService<MessageBroker>() as IHandlersRegistry);
    }
}
