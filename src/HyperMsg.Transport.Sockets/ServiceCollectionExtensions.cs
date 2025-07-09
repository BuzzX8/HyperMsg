using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Transport.Sockets;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientSocketTransport(this IServiceCollection services, EndPoint endPoint) => services.AddSingleton<ITransportContext>(services => new SocketTransport(CreateDefaultClientSocket(endPoint)));

    private static ISocket CreateDefaultClientSocket(EndPoint endPoint)
    {
        var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        return new SocketAdapter(socket, endPoint);
    }
}
