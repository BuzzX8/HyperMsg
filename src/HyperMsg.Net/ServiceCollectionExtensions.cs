using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;

namespace HyperMsg.Net;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSocket(this IServiceCollection services, Func<IServiceProvider, Socket> socketFactory) => services.AddSingleton(socketFactory);

    public static IServiceCollection AddClientSocketService(this IServiceCollection services) => services.AddHostedService<ClientSocketService>();
}
