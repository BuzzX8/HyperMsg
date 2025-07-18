﻿using HyperMsg.Buffers;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Transport.Sockets;

/// <summary>
/// Provides extension methods for registering socket transport services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a client socket transport as a singleton <see cref="ITransportContext"/> in the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the transport to.</param>
    /// <param name="endPoint">The remote endpoint to connect the socket to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddClientSocketTransport(this IServiceCollection services, EndPoint endPoint)
    {        
        return services.AddSingleton<ITransportContext>(services =>
        {
            var bufferingContext = services.GetService<IBufferingContext>();
            var socket = CreateDefaultClientSocket(endPoint);
            var transportContext = new SocketTransport(socket);

            if (bufferingContext is not null)
            {
                bufferingContext.OutputHandlers.Add(async (buffer, ctx) => 
                {
                    var data = buffer.Reader.GetMemory();
                    await transportContext.SendAsync(data, ctx);
                });
            }

            return transportContext;
        });
    }

    /// <summary>
    /// Creates a default client socket for the specified endpoint.
    /// </summary>
    /// <param name="endPoint">The remote endpoint to connect the socket to.</param>
    /// <returns>An <see cref="ISocket"/> instance configured for the endpoint.</returns>
    private static ISocket CreateDefaultClientSocket(EndPoint endPoint)
    {
        var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        return new SocketAdapter(socket, endPoint);
    }
}
