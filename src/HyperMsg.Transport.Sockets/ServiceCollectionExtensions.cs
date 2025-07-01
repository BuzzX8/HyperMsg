using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Transport.Sockets;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the socket transport services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add the transport services to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddSocketTransport(this IServiceCollection services)
    {
        // Register the socket transport as a singleton service
        services.AddSingleton<ITransportContext, SocketTransport>();
        return services;
    }
}
